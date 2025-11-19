using Api.Modelos.Enums;
using Api.Modelos.Request;

namespace Api.Modelos.Entidades
{
    public class MensalistaDia
    {
        public int Id { get; set; }
        public int MensalistaId { get; set; }
        public DayOfWeek DiaSemana { get; set; }
        public TimeOnly Horario { get; set; }
        public MensalistaDia()
        {
            
        }
        public MensalistaDia(MensalistaDiaRequest request)
        {
            DiaSemana = request.DiaSemana;
            Horario = request.Horario;
        }
    }
}
