using Api.Modelos.Enums;

namespace Api.Modelos.Request
{
    public class MensalistaDiaRequest
    {
        public required DayOfWeek DiaSemana { get; set; }
        public required TimeOnly Horario { get; set; }
    }
}
