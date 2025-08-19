namespace Api.Modelos.Entidades
{
    public class AgendamentoHorario
    {
        public int Id { get; set; }
        public int HorarioId { get; set; }           // FK para Horario
        public Horario Horario { get; set; }
    }
}
