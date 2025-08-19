namespace Api.Modelos.Entidades
{
    public class Horario
    {
        public int Id { get; set; }
        public TimeOnly Hora { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}
