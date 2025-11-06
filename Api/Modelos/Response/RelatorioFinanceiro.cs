namespace Api.Modelos.Response
{
    public class RelatorioFinanceiro
    {
        public int CortesRealizados { get; set; }
        public decimal ValorArrecadado { get; set; }
        public required string Barbeiro { get; set; }
        public List<RelatorioTop10Clientes> Top10Clientes { get; set; } = [];
        public List<RelatorioTop10Servicos> Top10Servicos { get; set; } = [];

    }
}
