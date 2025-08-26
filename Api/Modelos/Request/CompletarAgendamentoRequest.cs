using Api.Modelos.Entidades;
using Api.Modelos.Enums;

namespace Api.Modelos.Request
{
    public class CompletarAgendamentoRequest
    {
        public int IdAgendamento { get; set; }
        public List<int> IdsServico { get; set; }
        public MetodoPagamento MetodoPagamento { get; set; }
    }
}
