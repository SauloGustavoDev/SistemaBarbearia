using Api.Modelos.Enums;

namespace Api.Modelos.Request
{
    public class BarbeiroEditarRequest
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string Numero { get; set; }
        public required string Descricao { get; set; }
        public TipoAgenda Agenda { get; set; }
    }
}
