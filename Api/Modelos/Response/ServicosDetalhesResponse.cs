namespace Api.Modelos.Response
{
    public class ServicosDetalhesResponse
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public TimeOnly TempoEstimado { get; set; }
        public string Categoria { get; set; } = string.Empty;
    }
}
