namespace Api.Modelos.Request
{
    public class ServicoAtualizarRequest
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public TimeOnly TempoEstimado { get; set; }
        public int Categoria { get; set; }
    }
}
