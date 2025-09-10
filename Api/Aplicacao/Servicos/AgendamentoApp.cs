using Api.Aplicacao.Contratos;
using Api.Infraestrutura;
using Api.Infraestrutura.Contexto;
using Api.Modelos.Entidades;
using Api.Modelos.Enums;
using Api.Modelos.Request;
using Api.Modelos.Response;
using Microsoft.EntityFrameworkCore;

namespace Api.Aplicacao.Servicos
{
    public class AgendamentoApp : IAgendamentoApp
    {
        public readonly Contexto _contexto;
        public AgendamentoApp(Contexto contexto)
        {
            _contexto = contexto;
        }

        public GenericResponse CriarAgendamento(AgendamentoCriarRequest request)
        {
            try
            {
                request.DtAgendamento = request.DtAgendamento.ToUniversalTime();

                var horariosOcupados = _contexto.AgendamentoHorario
                    .Include(ah => ah.Agendamento)
                    .Where(ah => ah.Agendamento.DtAgendamento.Date == request.DtAgendamento.Date &&
                                 ah.Agendamento.IdBarbeiro == request.IdBarbeiro &&
                                 request.IdsHorario.Contains(ah.IdBarbeiroHorario))
                    .ToList();

                if (horariosOcupados.Any())
                {
                    var idsOcupados = string.Join(", ", horariosOcupados.Select(h => h.IdBarbeiroHorario));
                    return new GenericResponse { Sucesso = false, ErrorMessage = $"Os seguintes horários já estão ocupados: {idsOcupados}." };
                }

                var novoAgendamento = new Agendamento(request);
                _contexto.Agendamento.Add(novoAgendamento);
                _contexto.SaveChanges();

                return new GenericResponse { Sucesso = true };
            }
            catch
            {
                return new GenericResponse { Sucesso = false, ErrorMessage = "Ocorreu um erro inesperado ao criar o agendamento." };
            }
        }
        public List<AgendamentoResponse> ListarAgendamentos(int idBarbeiro, int idServico, string nomeCliente, DateTime? dtInicio, DateTime? dtFim, int status)
        {
            dtInicio = dtInicio.HasValue ? dtInicio : DateTime.Now.Date.ToUniversalTime();
            dtFim = dtFim.HasValue ? dtFim : DateTime.Now.Date.ToUniversalTime();


            var agendamentos = _contexto.Agendamento
                                  .AsNoTracking()
                                  .Where(x => x.IdBarbeiro == idBarbeiro &&
                                              x.DtAgendamento.Date.ToUniversalTime() >= dtInicio.Value.Date.ToUniversalTime() &&
                                              x.DtAgendamento.Date.ToUniversalTime() <= dtFim.Value.Date.ToUniversalTime() &&
                                              (nomeCliente == null || x.NomeCliente.ToUpper().Contains(nomeCliente.ToUpper())) &&
                                              (status == 0 || (int)x.Status == status) &&
                                              (idServico == 0 || x.AgendamentoServicos.Any(j => j.IdServico == idServico)))
                                  .Include(x => x.AgendamentoHorarios)
                                  .ThenInclude(x => x.BarbeiroHorario)
                                  .Include(x => x.AgendamentoServicos)
                                  .ThenInclude(x => x.Servico)
                                  .OrderByDescending(x => x.DtAgendamento)
                                  .GroupBy(x => x.DtAgendamento.Date)
                                  .ToList();

            var agendamentosResult = agendamentos
                .Select(g => new AgendamentoResponse(g.ToList(), g.Key))
                .ToList();

            return agendamentosResult;

        }

        public List<BarbeiroHorarioResponse> HorariosBarbeiro(BarbeiroHorarioRequest request)
        {
            var hoje = DateTime.Today;
            var diasParaGerar = 7 - DateTime.Now.DayOfWeek + 7; // semana atual + próxima semana
            var datasParaConsulta = Enumerable.Range(0, (int)diasParaGerar)
                                              .Select(i => hoje.AddDays(i).Date.ToUniversalTime())
                                              .Where(d => d.DayOfWeek != DayOfWeek.Sunday)
                                              .ToList();

            var horariosPadraoAtivos = _contexto.BarbeiroHorario
                .AsNoTracking()
                .Include(h => h.BarbeiroHorarioExcecao)
                .Where(h => h.IdBarbeiro == request.IdBarbeiro && h.DtFim == null)
                .ToList();

            var horariosOcupados = _contexto.Agendamento
                .AsNoTracking()
                .Where(a => a.IdBarbeiro == request.IdBarbeiro && !datasParaConsulta.Contains(a.DtAgendamento.Date.ToUniversalTime()))
                .SelectMany(a => a.AgendamentoHorarios)
                .Include(x => x.Agendamento)
                .ToList();

            var respostaFinal = new List<BarbeiroHorarioResponse>();

            foreach (var data in datasParaConsulta)
            {
                var horariosDoDia = horariosPadraoAtivos
                    .Where(h => h.TipoDia == (TipoDia)ValidaUtilOuSabado(data)).OrderBy(x => x.Hora).ToList();

                var horariosDisponiveis = new List<HorarioResponse>();

                foreach (var horario in horariosDoDia)
                {
                    bool temExcecao = horario.BarbeiroHorarioExcecao?.DtExcecao.Date.ToUniversalTime() == data.Date.ToUniversalTime();
                    bool estaOcupado = horariosOcupados.Any(x => x.Agendamento.DtAgendamento.Date.ToUniversalTime() == data.Date.ToUniversalTime() && x.IdBarbeiroHorario == horario.Id);

                    if (!temExcecao && !estaOcupado)
                    {
                        horariosDisponiveis.Add(new HorarioResponse
                        {
                            Id = horario.Id,
                            Hora = horario.Hora
                        });
                    }
                }

                var servicosSelecionados = _contexto.Servico
                                .AsNoTracking()
                                .Where(x => request.IdsServico.Contains(x.Id))
                                .ToList();

                var duracaoTotalServicos = TimeSpan.Zero;

                foreach (var servico in servicosSelecionados)
                {
                    duracaoTotalServicos += servico.TempoEstimado.ToTimeSpan();
                }

                int slotsNecessarios = (int)Math.Ceiling(duracaoTotalServicos.TotalMinutes / 40.0);

                if (slotsNecessarios <= 1)
                {
                    if (horariosDisponiveis.Any())
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

                if (horariosDeInicioValidos.Any())
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
                .ThenBy(x => x.Horarios.FirstOrDefault()?.Hora)
                .ToList();
        }

        private int ValidaUtilOuSabado(DateTime data)
        {
            return (data.DayOfWeek == DayOfWeek.Saturday) ? 2 : 1;
        }

        public GenericResponse CompletarAgendamento(AgendamentoCompletarRequest request)
        {
            var agendamento = _contexto.Agendamento
                   .FirstOrDefault(a => a.Id == request.IdAgendamento);

            if (agendamento == null)
                return new GenericResponse { Sucesso = false, ErrorMessage = "Falha ao atualizar agendamento" };

            agendamento.MetodoPagamento = request.MetodoPagamento;
            agendamento.Status = Status.Concluido;

            _contexto.SaveChanges();

            return new GenericResponse { Sucesso = true, ErrorMessage = "Agendamento completado!" };
        }

        public GenericResponse CancelarAgendamento(int id)
        {
            var agendamento = _contexto.Agendamento
                   .FirstOrDefault(a => a.Id == id);

            if (agendamento == null)
                return new GenericResponse { Sucesso = false, ErrorMessage = "Falha ao cancelar agendamento" };

            agendamento.Status = Status.CanceladoPeloBarbeiro;

            _contexto.SaveChanges();
            return new GenericResponse { Sucesso = true, ErrorMessage = "Agendamento cancelado!" };
        }

        public GenericResponse AtualizarAgendamento(AgendamentoAtualizarRequest agendamento)
        {
            try
            {
                var agendamentoExistente = _contexto.Agendamento
                    .Include(a => a.AgendamentoServicos)
                    .FirstOrDefault(a => a.Id == agendamento.Id);

                if (agendamentoExistente == null)
                {
                    return new GenericResponse { Sucesso = false, ErrorMessage = "Agendamento não encontrado." };
                }

                agendamentoExistente.MetodoPagamento = agendamento.MetodoPagamento;
                _contexto.AgendamentoServico.RemoveRange(agendamentoExistente.AgendamentoServicos);
                agendamentoExistente.AgendamentoServicos = agendamento.IdsServico.Select(x => new AgendamentoServico { IdAgendamento = agendamento.Id, IdServico = x }).ToList();
                _contexto.SaveChanges();
                return new GenericResponse { Sucesso = true };
            }
            catch
            {
                return new GenericResponse { Sucesso = false, ErrorMessage = "Ocorreu um erro inesperado ao atualizar o agendamento." };
            }
        }

        public AgendamentoAtualResponse AgendamentoAtual(int idBarbeiro)
        {
            var agendamento = _contexto.Agendamento
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
                                .Select(ah => ah.BarbeiroHorario.Hora)
                                .FirstOrDefault())
                            .FirstOrDefault();

            if (agendamento == null)
                throw new Exception("Nenhum horário agendado");

            var agendamentoAtual = new AgendamentoAtualResponse(agendamento);

            return agendamentoAtual;
        }
    }
}
