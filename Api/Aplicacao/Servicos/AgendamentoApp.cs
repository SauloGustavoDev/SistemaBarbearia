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

                return new GenericResponse { Sucesso = true};
                }
            catch (Exception ex)
            {
                return new GenericResponse { Sucesso = false, ErrorMessage = "Ocorreu um erro inesperado ao criar o agendamento." };
            }
        }

        public List<BarbeiroHorarioResponse> HorariosBarbeiro(BarbeiroHorarioRequest request)
        {
            var hoje = DateTime.Today;
            var diasParaGerar = 7 - DateTime.Now.DayOfWeek + 7; // semana atual + próxima semana
            var datasParaConsulta = Enumerable.Range(0, (int)diasParaGerar)
                                              .Select(i => hoje.AddDays(i))
                                              .Where(d => d.DayOfWeek != DayOfWeek.Sunday)
                                              .ToList();

            var horariosPadraoAtivos = _contexto.BarbeiroHorario
                .AsNoTracking()
                .Include(h => h.BarbeiroHorarioExcecao)
                .Where(h => h.IdBarbeiro == request.Id && h.DtFim == null)
                .ToList();

            var idsHorariosOcupados = _contexto.Agendamento
                .AsNoTracking()
                .Where(a => a.IdBarbeiro == request.Id && datasParaConsulta.Contains(a.DtAgendamento.Date))
                .SelectMany(a => a.AgendamentoHorarios) 
                .Select(ah => ah.IdBarbeiroHorario)     
                .ToHashSet();

            var respostaFinal = new List<BarbeiroHorarioResponse>();

            foreach (var data in datasParaConsulta)
            {
                var horariosDoDia = horariosPadraoAtivos
                    .Where(h => h.TipoDia == (TipoDia)ValidaUtilOuSabado(data));

                var horariosDisponiveis = new List<HorarioResponse>();

                foreach (var horario in horariosDoDia)
                {
                    bool temExcecao = horario.BarbeiroHorarioExcecao?.DtExcecao.Date == data.Date;
                    bool estaOcupado = idsHorariosOcupados.Contains(horario.Id);

                    if (!temExcecao && !estaOcupado)
                    {
                        horariosDisponiveis.Add(new HorarioResponse
                        {
                            Id = horario.Id,
                            Hora = horario.Hora
                        });
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

        private bool ValidaExcecaoHorario(BarbeiroHorario excecao, DateTime data)
        {
                return (excecao.BarbeiroHorarioExcecao == null) || (excecao.BarbeiroHorarioExcecao.DtExcecao != data);
        }

    }
}
