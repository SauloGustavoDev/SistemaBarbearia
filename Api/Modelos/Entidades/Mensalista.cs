using Api.Modelos.Enums;
using Api.Modelos.Request;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;

namespace Api.Modelos.Entidades
{
    public class Mensalista
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Numero { get; set; }
        public decimal Valor { get; set; }
        public required MensalistaDia Dia { get; set; }
        public TipoMensalista Tipo { get; set; }
        public DateTime DtInicio { get; set; }
        public DateTime? DtFim { get; set; }
        public StatusMensalista Status { get; set; }
        public int IdBarbeiro { get; set; }

        public Mensalista()
        {
            
        }
        [SetsRequiredMembers]
        public Mensalista(MensalistaCriarRequest request)
        {
            Nome = request.Nome;
            Valor = request.Valor;
            Tipo = request.Tipo;
            DtInicio = DateTime.UtcNow;
            Dia = new MensalistaDia(request.Dia);
            Status = request.Status;
            Numero = request.Numero;
        }
    }
}
