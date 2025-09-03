using Api.Aplicacao.Contratos;
using Api.Infraestrutura;
using Api.Infraestrutura.Contexto;
using Api.Modelos.Entidades;
using Api.Modelos.Enums;
using Api.Modelos.Request;
using Api.Modelos.Response;
using Api.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            catch (Exception ex)
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
                .Where(h => h.IdBarbeiro == request.Id && h.DtFim == null)
                .ToList();

            var horariosOcupados = _contexto.Agendamento
                .AsNoTracking()
                .Where(a => a.IdBarbeiro == request.Id && !datasParaConsulta.Contains(a.DtAgendamento.Date.ToUniversalTime()))
                .SelectMany(a => a.AgendamentoHorarios)
                .Include(x => x.Agendamento)
                .ToList();

            var respostaFinal = new List<BarbeiroHorarioResponse>();

            foreach (var data in datasParaConsulta)
            {
                var horariosDoDia = horariosPadraoAtivos
                    .Where(h => h.TipoDia == (TipoDia)ValidaUtilOuSabado(data));

                var horariosDisponiveis = new List<HorarioResponse>();

                foreach (var horario in horariosDoDia)
                {
                    bool temExcecao = horario.BarbeiroHorarioExcecao?.DtExcecao.Date.ToUniversalTime() == data.Date.ToUniversalTime();
                    bool estaOcupado = horariosOcupados.Any(x => x.Agendamento.DtAgendamento.ToUniversalTime() == data.ToUniversalTime() && x.IdBarbeiroHorario == horario.Id);

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
                    // TimeOnly não pode ser somado diretamente, então convertemos para TimeSpan.
                    duracaoTotalServicos += servico.TempoEstimado.ToTimeSpan();
                }

                if (duracaoTotalServicos > TimeSpan.FromMinutes(40))
                {

                    for (int j = 0; j < horariosDisponiveis.Count - 1; j++)
                    {
                        if (horariosDisponiveis[j].Hora.ToTimeSpan() == TimeSpan.FromHours(12) && data.DayOfWeek != DayOfWeek.Saturday)
                        {
                            if (horariosDisponiveis[j + 1].Hora.ToTimeSpan() != new TimeSpan(13, 20, 0) && duracaoTotalServicos > TimeSpan.FromMinutes(80))
                            {
                                horariosDisponiveis.RemoveAt(j);
                                break;
                            }
                        }

                        if (horariosDisponiveis[j].Hora.ToTimeSpan() == new TimeSpan(12, 20, 0) && data.DayOfWeek == DayOfWeek.Saturday)
                        {
                            if (horariosDisponiveis[j].Hora.AddMinutes(60) != horariosDisponiveis[j + 1].Hora && duracaoTotalServicos > TimeSpan.FromMinutes(60))
                            {
                                horariosDisponiveis.RemoveAt(j);
                                break;
                            }
                        }

                        if (horariosDisponiveis[j].Hora.AddMinutes(40) != horariosDisponiveis[j + 1].Hora && duracaoTotalServicos > TimeSpan.FromMinutes(40))
                        {
                            horariosDisponiveis.RemoveAt(j);
                            break;
                        }
                    }
                }
                if (horariosDisponiveis.Any())
                {
                    respostaFinal.Add(new BarbeiroHorarioResponse
                    {
                        Data = data,
                        Horarios = horariosDisponiveis
                    });
                }
            }

            return respostaFinal;

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
            catch (Exception ex)
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
                .Include(a => a.AgendamentoServicos)
                .ThenInclude(a => a.Servico)
                .OrderBy(a => a.DtAgendamento)
                .FirstOrDefault();
            
            var agendamentoAtual = new AgendamentoAtualResponse(agendamento);

            return agendamentoAtual;
        }
    }
}
