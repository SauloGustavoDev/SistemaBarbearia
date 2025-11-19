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

        public void GerarToken(string numero)
        {
            var tokenAtivo =  _contexto.TokenConfirmacao
                .FirstOrDefault(t => t.Numero == numero && !t.Confirmado && t.DtExpiracao.ToUniversalTime() > DateTime.UtcNow);

            var ultimoAgendamento = _contexto.Agendamento
                .Where(x => x.NumeroCliente == numero && x.Status == Status.Concluido)
                .OrderByDescending(x => x.DtAgendamento)
                .FirstOrDefault();

            if (ultimoAgendamento != null && ultimoAgendamento.DtAgendamento.AddDays(7) >= DateTime.UtcNow)
                throw new Exception("Seu ultimo corte foi a menos de 7 dias");

            if (tokenAtivo != null && tokenAtivo.Reenviado)
                throw new Exception("Já foi enviado um código de confirmação. Por favor, verifique seu telefone.");

            if (tokenAtivo != null && !tokenAtivo.Reenviado)
            {
                HelperGenerico.EnviarMensagem(tokenAtivo.Codigo.ToString(), numero);
                tokenAtivo.Reenviado = true;
                _contexto.SaveChanges();
                return;
            }

            var codigo = HelperGenerico.GerarCodigoConfirmacao();
            var salvarToken = new CodigoConfirmacao(numero, codigo);

            _contexto.TokenConfirmacao.Add(salvarToken);
            _contexto.SaveChanges();
            HelperGenerico.EnviarMensagem(codigo.ToString(), numero);
        }

        public void CriarAgendamento(AgendamentoCriarRequest request)
        {
            var tokenValido =  _contexto.TokenConfirmacao
                .FirstOrDefault(t => t.Numero == request.Numero 
                                    && t.Codigo == request.CodigoConfirmacao 
                                    && !t.Confirmado 
                                    && t.DtExpiracao.ToUniversalTime() > DateTime.UtcNow) ?? throw new Exception("Código de confirmação inválido ou expirado.");

            tokenValido.Confirmado = true;

            request.DtAgendamento = request.DtAgendamento.ToUniversalTime();

            var horariosOcupados =  _contexto.AgendamentoHorario
                .Include(ah => ah.Agendamento)
                .Where(ah => ah.Agendamento!.DtAgendamento.Date == request.DtAgendamento.Date &&
                             ah.Agendamento.IdBarbeiro == request.IdBarbeiro &&
                             request.IdsHorario.Contains(ah.IdBarbeiroHorario))
                .ToList();

            if (request.IdsHorario.Count == 0)
                throw new Exception("Nenhum horario selecionado");

            if (horariosOcupados.Count != 0)
            {
                var idsOcupados = string.Join(", ", horariosOcupados.Select(h => h.IdBarbeiroHorario));
                throw new Exception($"Os seguintes horários já estão ocupados: {idsOcupados}");
            }

            var novoAgendamento = new Agendamento(request);
            _contexto.Agendamento.Add(novoAgendamento);
            _contexto.SaveChanges();
        }
        public ResultadoPaginado<AgendamentosDetalheResponse> ListarAgendamentos(AgendamentoListarRequest request)
        {
            request.DtInicio = request.DtInicio.HasValue ? request.DtInicio : null;
            request.DtFim = request.DtFim.HasValue ? request.DtFim : null;



            var query = _contexto.Agendamento
                                  .AsNoTracking()
                                  .Where(x => x.IdBarbeiro == request.IdBarbeiro &&
                                               (request.DtInicio == null || x.DtAgendamento.Date.ToUniversalTime() >= request.DtInicio.Value.Date.ToUniversalTime()) &&
                                               (request.DtFim == null|| x.DtAgendamento.Date.ToUniversalTime() <= request.DtFim.Value.Date.ToUniversalTime()) &&
                                              (request.NomeCliente == null || x.NomeCliente.Contains(request.NomeCliente, StringComparison.CurrentCultureIgnoreCase)) &&
                                              (request.Status == null || x.Status == request.Status) &&
                                              (request.IdServico == null || x.AgendamentoServicos.Any(j => j.IdServico == request.IdServico)))
                                  .Include(x => x.AgendamentoHorarios)
                                  .ThenInclude(x => x.BarbeiroHorario)
                                  .Include(x => x.AgendamentoServicos)
                                  .ThenInclude(x => x.Servico)
                                  .OrderBy(x => x.DtAgendamento)
                                  .Select(x => new AgendamentosDetalheResponse(x))
                                  .AsQueryable();

            return Paginacao.CriarPaginacao(query, request.Pagina, request.ItensPorPagina);
        }

        public  List<BarbeiroHorarioResponse> HorariosBarbeiro(BarbeiroHorarioRequest request)
        {
            var hoje = DateTime.Today;

            var agenda = _contexto.Barbeiro
                .Find(request.IdBarbeiro)!
                .Agenda;

            var diasParaGerar = HelperGenerico.GerarDiasAgenda(agenda); // semana atual + próxima semana
            var datasParaConsulta = Enumerable.Range(0, diasParaGerar)
                                              .Select(i => hoje.AddDays(i).Date.ToUniversalTime())
                                              .Where(d => d.DayOfWeek != DayOfWeek.Sunday)
                                              .ToList();

            var horariosPadraoAtivos =  _contexto.BarbeiroHorario
                .AsNoTracking()
                .Include(h => h.BarbeiroHorarioExcecao)
                .Where(h => h.IdBarbeiro == request.IdBarbeiro && h.DtFim == null)
                .ToList();

            var horariosOcupados =  _contexto.Agendamento
                .AsNoTracking()
                .Where(a => a.IdBarbeiro == request.IdBarbeiro && !datasParaConsulta.Contains(a.DtAgendamento.Date.ToUniversalTime()))
                .SelectMany(a => a.AgendamentoHorarios)
                .Include(x => x.Agendamento)
                .ToList();

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

                var servicosSelecionados =  _contexto.Servico
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

        public void CompletarAgendamento(AgendamentoCompletarRequest request)
        {
            var agendamento = _contexto.Agendamento.Find(request.IdAgendamento) ?? throw new Exception("Agendamento não encontrado");
            agendamento.MetodoPagamento = request.MetodoPagamento;
            agendamento.Status = Status.Concluido;
            _contexto.SaveChanges();
        }

        public void CancelarAgendamento(int id)
        {
            var agendamento = _contexto.Agendamento.Find(id) ?? throw new Exception("Agendamento não encontrado");
            agendamento.Status = Status.CanceladoPeloBarbeiro;
            _contexto.SaveChanges();
        }
        public void AtualizarAgendamento(AgendamentoAtualizarRequest agendamento)
        {
            var agendamentoExistente =  _contexto.Agendamento
                .Include(a => a.AgendamentoServicos)
                .FirstOrDefault(a => a.Id == agendamento.Id);

            if (agendamentoExistente == null)
            {
                throw new Exception("Agendamento não encontrado");
            }

            agendamentoExistente.MetodoPagamento = agendamento.MetodoPagamento;
            _contexto.AgendamentoServico.RemoveRange(agendamentoExistente.AgendamentoServicos);
            agendamentoExistente.AgendamentoServicos = agendamento.IdsServico.Select(x => new AgendamentoServico { IdAgendamento = agendamento.Id, IdServico = x }).ToList();
            _contexto.SaveChanges();
        }
        public  AgendamentoAtualResponse AgendamentoAtual(int idBarbeiro)
        {
            var agendamento =  _contexto.Agendamento
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
                            .FirstOrDefault() ?? throw new Exception("Nenhum horário agendado");
            var agendamentoAtual = new AgendamentoAtualResponse(agendamento);

            return agendamentoAtual;
        }
    }
}
