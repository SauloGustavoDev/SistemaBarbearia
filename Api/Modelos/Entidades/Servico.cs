using Api.Modelos.Enums;

namespace Api.Modelos.Entidades
{
    public class Servico
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public TimeOnly TempoEstimado { get; set; }
        public DateTime DtInicio { get; set; }
        public DateTime? DtFim { get; set; }

    }
}
