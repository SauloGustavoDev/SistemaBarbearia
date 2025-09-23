namespace Api.Modelos.Entidades
{
    public class AgendamentoServico
    {
        public int IdAgendamento { get; set; }
        public Agendamento? Agendamento { get; set; }

        public int IdServico { get; set; }
        public Servico? Servico { get; set; }
    }
}
