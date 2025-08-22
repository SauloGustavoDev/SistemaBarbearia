namespace Api.Modelos.Request
{
    public class AgendamentoCriarRequest
    {
        public int IdBarbeiro { get; set; }
        public List<int> IdsServico { get; set; }
        public DateTime DtAgendamento { get; set; }
        public List<int> IdsHorario { get; set; }
        public string Numero { get; set; }
        public string Nome { get; set; }
    }
}
