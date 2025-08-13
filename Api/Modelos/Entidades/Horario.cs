namespace Api.Modelos.Entidades
{
    public class Horario
    {
        private int Id { get; set; }
        private TimeOnly Hora { get; set; }
        private DateTime DataInicio { get; set; }
        private DateTime DataFim { get; set; }
    }
}
