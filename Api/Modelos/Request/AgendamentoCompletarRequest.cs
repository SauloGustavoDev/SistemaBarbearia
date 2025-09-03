using Api.Modelos.Entidades;
using Api.Modelos.Enums;

namespace Api.Modelos.Request
{
    public class AgendamentoCompletarRequest
    {
        public int IdAgendamento { get; set; }
        public MetodoPagamento MetodoPagamento { get; set; }
    }
}
