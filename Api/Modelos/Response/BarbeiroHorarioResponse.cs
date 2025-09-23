using Api.Modelos.Entidades;

namespace Api.Modelos.Response
{
    public class BarbeiroHorarioResponse
    {
        public DateTime Data { get; set; }
        public List<HorarioResponse>? Horarios { get; set; }
    }
}
