using Api.Modelos.Entidades;
using Api.Modelos.Enums;

namespace Api.Modelos.Response
{
    public class AgendamentosDetalheResponse
    {
        public int Id { get; set; }
        public string NomeCliente { get; set; }
        public string NumeroCliente { get; set; }
        public Status Status { get; set; }
        public List<int> IdHorarios{ get; set; }
        public List<TimeOnly> Horario { get; set; }
        public List<int> IdServicos { get; set; }
        public List<Servico> Servicos { get; set; }

        public AgendamentosDetalheResponse(Agendamento request)
        {
            Id = request.Id;
            NomeCliente = request.NomeCliente;
            NumeroCliente = request.NumeroCliente;
            Status = request.Status;
            IdHorarios = request.AgendamentoHorarios.Select(x => x.IdBarbeiroHorario).ToList();
            IdServicos = request.AgendamentoServicos.Select(x => x.IdServico).ToList();
        }
    }
}
