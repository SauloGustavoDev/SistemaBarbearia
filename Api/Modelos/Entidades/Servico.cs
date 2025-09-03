using Api.Modelos.Enums;
using Api.Models.Entity;

namespace Api.Modelos.Entidades
{
    public class Servico
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public CategoriaServico CategoriaServico { get; set; }
        public TimeOnly TempoEstimado { get; set; }
        public DateTime DtInicio { get; set; }
        public DateTime? DtFim { get; set; }
        public int IdBarbeiro { get; set; }
    }
}
