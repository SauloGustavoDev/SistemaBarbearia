namespace Api.Modelos.Entidades
{
    public class Servico
    {
        public int Id { get; set; }
        public required string Descricao { get; set; }
        public decimal Valor { get; set; }
        public TimeOnly TempoEstimado { get; set; }
        public DateTime DtInicio { get; set; }
        public DateTime? DtFim { get; set; }
        public int IdCategoriaServico { get; set; }
        public CategoriaServico? CategoriaServico { get; set; }
        public List<AgendamentoServico> AgendamentoServicos { get; set; } = [];

    }
}
