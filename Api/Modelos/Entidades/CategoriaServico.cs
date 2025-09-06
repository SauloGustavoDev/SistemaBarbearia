namespace Api.Modelos.Entidades
{
    public class CategoriaServico
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public DateTime DtInicio { get; set; }
        public DateTime? DtFim { get; set; }
        public List<Servico> Servicos { get; set; } = new List<Servico>();
    }
}
