using Api.Aplicacao.Contratos;
using Api.Aplicacao.Helpers;
using Api.Infraestrutura.Contexto;
using Api.Modelos.Entidades;
using Api.Modelos.Enums;
using Api.Modelos.Request;
using Api.Modelos.Response;
using Microsoft.EntityFrameworkCore;

namespace Api.Aplicacao.Servicos
{
    public class AgendamentoApp(Contexto contexto) : IAgendamentoApp
    {
        public readonly Contexto _contexto = contexto;

        public async Task<GenericResponse> CriarAgendamento(AgendamentoCriarRequest request)
        {
            request.DtAgendamento = request.DtAgendamento.ToUniversalTime();

            var horariosOcupados = await _contexto.AgendamentoHorario
                .Include(ah => ah.Agendamento)
                .Where(ah => ah.Agendamento!.DtAgendamento.Date == request.DtAgendamento.Date &&
                             ah.Agendamento.IdBarbeiro == request.IdBarbeiro &&
                             request.IdsHorario.Contains(ah.IdBarbeiroHorario))
                .ToListAsync();

            if (horariosOcupados.Count != 0)
            {
                var idsOcupados = string.Join(", ", horariosOcupados.Select(h => h.IdBarbeiroHorario));
                return new GenericResponse { Sucesso = false, ErrorMessage = $"Os seguintes horários já estão ocupados: {idsOcupados}." };
            }

            var novoAgendamento = new Agendamento(request);



            return await MontarGenericResponse.TryExecuteAsync(async () =>
            {
                await _contexto.Agendamento.AddAsync(novoAgendamento);
                await _contexto.SaveChangesAsync();
            }, "Ocorreu um erro inesperado ao criar o agendamento.");

        }
        public async Task<ResultadoPaginado<AgendamentosDetalheResponse>> ListarAgendamentos(AgendamentoListarRequest request)
        {
            request.DtInicio = request.DtInicio.HasValue ? request.DtInicio : DateTime.Now.Date.ToUniversalTime();
            request.DtFim = request.DtFim.HasValue ? request.DtFim : DateTime.Now.Date.ToUniversalTime();


            var query = _contexto.Agendamento
                                  .AsNoTracking()
                                  .Where(x => x.IdBarbeiro == request.IdBarbeiro &&
                                              x.DtAgendamento.Date.ToUniversalTime() >= request.DtInicio.Value.Date.ToUniversalTime() &&
                                              x.DtAgendamento.Date.ToUniversalTime() <= request.DtFim.Value.Date.ToUniversalTime() &&
                                              (request.NomeCliente == null || x.NomeCliente.Contains(request.NomeCliente, StringComparison.CurrentCultureIgnoreCase)) &&
                                              (request.Status == null || x.Status == request.Status) &&
                                              (request.IdServico == 0 || x.AgendamentoServicos.Any(j => j.IdServico == request.IdServico)))
                                  .Include(x => x.AgendamentoHorarios)
                                  .ThenInclude(x => x.BarbeiroHorario)
                                  .Include(x => x.AgendamentoServicos)
                                  .ThenInclude(x => x.Servico)
                                  .OrderByDescending(x => x.DtAgendamento)
                                  .Select(x => new AgendamentosDetalheResponse(x))
                                  .AsQueryable();

            return await Paginacao.CriarPaginacao(query, request.Pagina, request.ItensPorPagina);
        }

        public async Task<List<BarbeiroHorarioResponse>> HorariosBarbeiro(BarbeiroHorarioRequest request)
        {
            var hoje = DateTime.Today;
            var diasParaGerar = 7 - (int)DateTime.Now.DayOfWeek + 7; // semana atual + próxima semana
            var datasParaConsulta = Enumerable.Range(0, diasParaGerar)
                                              .Select(i => hoje.AddDays(i).Date.ToUniversalTime())
                                              .Where(d => d.DayOfWeek != DayOfWeek.Sunday)
                                              .ToList();

            var horariosPadraoAtivos = await _contexto.BarbeiroHorario
                .AsNoTracking()
                .Include(h => h.BarbeiroHorarioExcecao)
                .Where(h => h.IdBarbeiro == request.IdBarbeiro && h.DtFim == null)
                .ToListAsync();

            var horariosOcupados = await _contexto.Agendamento
                .AsNoTracking()
                .Where(a => a.IdBarbeiro == request.IdBarbeiro && !datasParaConsulta.Contains(a.DtAgendamento.Date.ToUniversalTime()))
                .SelectMany(a => a.AgendamentoHorarios)
                .Include(x => x.Agendamento)
                .ToListAsync();

            var respostaFinal = new List<BarbeiroHorarioResponse>();

            foreach (var data in datasParaConsulta)
            {
                var horariosDoDia = horariosPadraoAtivos
                    .Where(h => h.TipoDia == (TipoDia)ValidaUtilOuSabado(data))
                    .OrderBy(x => x.Hora)
                    .ToList();

                var horariosDisponiveis = new List<HorarioResponse>();

                foreach (var horario in horariosDoDia)
                {
                    bool temExcecao = horario.BarbeiroHorarioExcecao?.DtExcecao.Date.ToUniversalTime() == data.Date.ToUniversalTime();
                    bool estaOcupado = horariosOcupados.Any(x => x.Agendamento!.DtAgendamento.Date.ToUniversalTime() == data.Date.ToUniversalTime()
                                                                 && x.IdBarbeiroHorario == horario.Id);

                    if (!temExcecao && !estaOcupado)
                    {
                        horariosDisponiveis.Add(new HorarioResponse
                        {
                            Id = horario.Id,
                            Hora = horario.Hora
                        });
                    }
                }

                var servicosSelecionados = await _contexto.Servico
                                .AsNoTracking()
                                .Where(x => request.IdsServico.Contains(x.Id))
                                .ToListAsync();

                var duracaoTotalServicos = TimeSpan.Zero;

                foreach (var servico in servicosSelecionados)
                {
                    duracaoTotalServicos += servico.TempoEstimado.ToTimeSpan();
                }

                int slotsNecessarios = (int)Math.Ceiling(duracaoTotalServicos.TotalMinutes / 40.0);

                if (slotsNecessarios <= 1)
                {
                    if (horariosDisponiveis.Count != 0)
                    {
                        respostaFinal.Add(new BarbeiroHorarioResponse
                        {
                            Data = data,
                            Horarios = horariosDisponiveis
                        });
                    }
                    continue;
                }

                var horariosDeInicioValidos = new List<HorarioResponse>();

                for (int i = 0; i < horariosDisponiveis.Count - slotsNecessarios + 1; i++)
                {
                    var horarioDeInicioPotencial = horariosDisponiveis[i];
                    bool sequenciaContinua = true;

                    for (int j = 0; j < slotsNecessarios - 1; j++)
                    {
                        var horarioAtual = horariosDisponiveis[i + j];
                        var proximoHorario = horariosDisponiveis[i + j + 1];

                        if (horarioAtual.Hora.AddMinutes(40) != proximoHorario.Hora)
                        {
                            bool ehPausaAlmoco = data.DayOfWeek != DayOfWeek.Saturday &&
                                                  horarioAtual.Hora.ToTimeSpan() == new TimeSpan(12, 0, 0) &&
                                                  proximoHorario.Hora.ToTimeSpan() == new TimeSpan(13, 20, 0);

                            if (!ehPausaAlmoco)
                            {
                                sequenciaContinua = false;
                                break;
                            }
                        }
                    }

                    if (sequenciaContinua)
                    {
                        horariosDeInicioValidos.Add(horarioDeInicioPotencial);
                    }
                }

                if (horariosDeInicioValidos.Count != 0)
                {
                    respostaFinal.Add(new BarbeiroHorarioResponse
                    {
                        Data = data,
                        Horarios = horariosDeInicioValidos
                    });
                }
            }

            return respostaFinal
                .OrderBy(x => x.Data)
                .ThenBy(x => x.Horarios?.FirstOrDefault()?.Hora)
                .ToList();
        }

        private static int ValidaUtilOuSabado(DateTime data)
        {
            return (data.DayOfWeek == DayOfWeek.Saturday) ? 2 : 1;
        }

        public async Task<GenericResponse> CompletarAgendamento(AgendamentoCompletarRequest request)
        {
            var agendamento = await _contexto.Agendamento
                   .FirstOrDefaultAsync(a => a.Id == request.IdAgendamento);

            if (agendamento == null)
                return new GenericResponse { Sucesso = false, ErrorMessage = "Falha ao atualizar agendamento" };

            agendamento.MetodoPagamento = request.MetodoPagamento;
            agendamento.Status = Status.Concluido;

            return await MontarGenericResponse.TryExecuteAsync(async () =>
            {
                await _contexto.SaveChangesAsync();
            }, "Falha ao completar o agendamento.");
        }

        public async Task<GenericResponse> CancelarAgendamento(int id)
        {
            var agendamento = await _contexto.Agendamento
                   .FirstOrDefaultAsync(a => a.Id == id);

            if (agendamento == null)
                return new GenericResponse { Sucesso = false, ErrorMessage = "Falha ao cancelar agendamento" };

            agendamento.Status = Status.CanceladoPeloBarbeiro;

            return await MontarGenericResponse.TryExecuteAsync(async () =>
            {
                await _contexto.SaveChangesAsync();
            }, "Falha ao cancelar agendamento.");
        }

        public async Task<GenericResponse> AtualizarAgendamento(AgendamentoAtualizarRequest agendamento)
        {
            var agendamentoExistente = await _contexto.Agendamento
                .Include(a => a.AgendamentoServicos)
                .FirstOrDefaultAsync(a => a.Id == agendamento.Id);

            if (agendamentoExistente == null)
            {
                return new GenericResponse { Sucesso = false, ErrorMessage = "Agendamento não encontrado." };
            }

            agendamentoExistente.MetodoPagamento = agendamento.MetodoPagamento;
            _contexto.AgendamentoServico.RemoveRange(agendamentoExistente.AgendamentoServicos);
            agendamentoExistente.AgendamentoServicos = agendamento.IdsServico.Select(x => new AgendamentoServico { IdAgendamento = agendamento.Id, IdServico = x }).ToList();
            return await MontarGenericResponse.TryExecuteAsync(async () =>
            {
                await _contexto.SaveChangesAsync();
            }, "Falha ao atualizar agendamento");
        }

        public async Task<AgendamentoAtualResponse> AgendamentoAtual(int idBarbeiro)
        {
            var agendamento = await _contexto.Agendamento
                             .AsNoTracking()
                             .Where(a => a.IdBarbeiro == idBarbeiro &&
                             (a.Status == Status.Pendente || a.Status == Status.Confirmado))
                             .Include(a => a.Barbeiro)
                             .Include(a => a.AgendamentoHorarios)
                             .ThenInclude(b => b.BarbeiroHorario)
                             .Include(a => a.AgendamentoServicos)
                             .ThenInclude(a => a.Servico)
                             .OrderBy(a => a.DtAgendamento)
                            .ThenBy(a => a.AgendamentoHorarios
                                .Select(ah => ah.BarbeiroHorario!.Hora)
                                .FirstOrDefault())
                            .FirstOrDefaultAsync() ?? throw new Exception("Nenhum horário agendado");
            var agendamentoAtual = new AgendamentoAtualResponse(agendamento);

            return agendamentoAtual;
        }
    }
}
