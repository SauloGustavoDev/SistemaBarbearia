namespace Api.Modelos.Response
{
    public class RelatorioTop10Clientes
    {
        public required string Nome { get; set; }
        public required string Numero { get; set; }
        public int TotalCortes { get; set; }
        public DateTime UltimoCorte { get; set; }
    }
}
