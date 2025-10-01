namespace Api.Modelos.Request
{
    public class AgendamentoCriarRequest
    {
        public int IdBarbeiro { get; set; }
        public required List<int> IdsServico { get; set; }
        public DateTime DtAgendamento { get; set; }
        public required List<int> IdsHorario { get; set; }
        public required string Numero { get; set; }
        public required string Nome { get; set; }
        public required int CodigoConfirmacao { get; set; }
    }
}
