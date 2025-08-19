using Api.Modelos.Enums;

namespace Api.Modelos.Entidades
{
    public class BarbeiroHorario
    {
        public int Id { get; set; }
        public TimeOnly Hora { get; set; }
        public DiaSemana DiaSemana { get; set; }
        public DateTime DtInicio { get; set; }
        public DateTime DtFim { get; set; }
    }
}
