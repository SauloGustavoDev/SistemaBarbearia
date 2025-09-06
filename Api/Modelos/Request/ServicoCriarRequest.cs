namespace Api.Modelos.Request
{
    public class ServicoCriarRequest
    {
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public int TempoEstimado { get; set; }
        public int Categoria { get; set; }
    }
}
