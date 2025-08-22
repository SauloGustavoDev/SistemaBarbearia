using Api.Modelos.Entidades;

namespace Api.Modelos.Response
{
    public class AgendamentoResponse
    {
        public DateTime? Data { get; set; }
        public List<AgendamentosDetalheResponse>? Agendamentos { get; set; } = new List<AgendamentosDetalheResponse>();

        public AgendamentoResponse(){}
        public AgendamentoResponse(List<Agendamento> request)
        {
            Data = request.FirstOrDefault()?.DtAgendamento;
            foreach (var item in request)
            {
                Agendamentos.Add(new AgendamentosDetalheResponse(item));
            }
        }
    }
}
