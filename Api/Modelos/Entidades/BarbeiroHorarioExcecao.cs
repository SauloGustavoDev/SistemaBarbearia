using Api.Modelos.Enums;

namespace Api.Modelos.Entidades
{
    public class BarbeiroHorarioExcecao
    {
        public int Id { get; set; }
        public DateTime DtExcecao { get; set; }
        public MotivoExcecao MotivoExcecao { get; set; }

        public int BarbeiroHorarioId { get; set; }  // FK explícita
        public BarbeiroHorario BarbeiroHorario { get; set; }
    }
}
