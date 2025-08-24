using Api.Modelos.Entidades;

namespace Api.Modelos.Response
{
    public class AgendamentoResponse
    {
        public DateTime? Data { get; set; }
        public List<AgendamentosDetalheResponse> Agendamentos { get; set; } = new();

        public AgendamentoResponse() { }

        public AgendamentoResponse(List<Agendamento> request, DateTime? data)
        {
            Data = data;
            Agendamentos = request.Select(a => new AgendamentosDetalheResponse(a)).ToList();
        }
    }
}
