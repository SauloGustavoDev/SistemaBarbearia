using Api.Modelos.Enums;
using Api.Modelos.Paginacao;

namespace Api.Modelos.Request
{
    public class AgendamentoListarRequest : PaginacaoFiltro
    {
        public int? IdBarbeiro { get; set; }
        public int? IdServico { get; set; }
        public string? NomeCliente { get; set; }
        public DateTime? DtInicio { get; set; }
        public DateTime? DtFim { get; set; }
        public Status? Status { get; set; }
    }
}
