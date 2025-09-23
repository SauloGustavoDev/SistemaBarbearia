namespace Api.Modelos.Entidades
{
    public class BarbeiroServico
    {
        public int Id { get; set; }
        public DateTime DtInicio { get; set; }
        public DateTime? DtFim { get; set; }
        // Propriedades de chave estrangeira
        public int IdBarbeiro { get; set; }
        public int IdServico { get; set; }

        // Propriedades de navegação
        public Barbeiro? Barbeiro { get; set; }
        public Servico? Servico { get; set; }
    }
}
