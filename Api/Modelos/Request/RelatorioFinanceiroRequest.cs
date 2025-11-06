namespace Api.Modelos.Request
{
    public class RelatorioFinanceiroRequest
    {
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public int IdBarbeiro { get; set; }
    }
}
