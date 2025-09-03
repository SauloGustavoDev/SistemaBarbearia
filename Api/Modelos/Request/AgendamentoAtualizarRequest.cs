using Api.Modelos.Enums;

namespace Api.Modelos.Request
{
    public class AgendamentoAtualizarRequest
    {
        public int Id { get; set; }
        public MetodoPagamento MetodoPagamento { get; set; }
        public List<int> IdsServico { get; set; } = new List<int>();
    }
}
