namespace Api.Modelos.Entidades
{
    public class AgendamentoHorario
    {
        public int Id { get; set; }
        public int IdAgendamento { get; set; }
        public int IdBarbeiroHorario { get; set; }

        // Navegações opcionais
        public Agendamento? Agendamento { get; set; }
        public BarbeiroHorario? BarbeiroHorario { get; set; }
    }
}
