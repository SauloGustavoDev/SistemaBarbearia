using Api.Modelos.Enums;
using Api.Modelos.Request;

namespace Api.Modelos.Entidades
{
    public class Agendamento
    {
        public int Id { get; set; }
        public int IdBarbeiro { get; set; }
        public string NomeCliente { get; set; } = string.Empty;
        public string NumeroCliente { get; set; } = string.Empty;
        public DateTime DtAgendamento { get; set; }
        public Status Status { get; set; }
        public MetodoPagamento MetodoPagamento { get; set; }
        public Barbeiro? Barbeiro { get; set; }
        public List<AgendamentoHorario> AgendamentoHorarios { get; set; } = [];
        public List<AgendamentoServico> AgendamentoServicos { get; set; } = [];
        public Agendamento() { }

        public Agendamento(AgendamentoCriarRequest request)
        {
            IdBarbeiro = request.IdBarbeiro;
            NomeCliente = request.Nome;
            NumeroCliente = request.Numero;
            DtAgendamento = request.DtAgendamento.ToUniversalTime();
            Status = Status.Pendente;

            AgendamentoHorarios = request.IdsHorario.Select(idHorario => new AgendamentoHorario
            {
                IdBarbeiroHorario = idHorario,
            }).ToList();

            AgendamentoServicos = request.IdsServico.Select(idServico => new AgendamentoServico
            {
                IdServico = idServico
            }).ToList();
        }
    }
}
