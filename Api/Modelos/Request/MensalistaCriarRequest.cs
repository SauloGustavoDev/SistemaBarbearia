using Api.Modelos.Entidades;
using Api.Modelos.Enums;

namespace Api.Modelos.Request
{
    public class MensalistaCriarRequest
    {
        public required string Nome { get; set; }
        public required string Numero { get; set; }
        public decimal Valor { get; set; }
        public required MensalistaDiaRequest Dia { get; set; }
        public required TipoMensalista Tipo { get; set; }
        public required StatusMensalista Status { get; set; }
    }
}
