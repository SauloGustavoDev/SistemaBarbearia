using Api.Modelos.Enums;

namespace Api.Modelos.Entidades
{
    public class BarbeiroHorario
    {
        public int Id { get; set; }
        public TimeOnly Hora { get; set; }
        public TipoDia TipoDia { get; set; }
        public DateTime DtInicio { get; set; }
        public DateTime? DtFim { get; set; }
        // 🟢 Adicionar a propriedade da chave estrangeira
        public int IdBarbeiro { get; set; }
        public BarbeiroHorarioExcecao? BarbeiroHorarioExcecao { get; set; }
    }
}
