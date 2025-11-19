using Api.Modelos.Entidades;
using Api.Modelos.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Api.Modelos.Response
{
    public class MensalistaDetalhesResponse
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Celular { get; set; }
        public decimal Valor { get; set; }
        public TipoMensalista Tipo { get; set; }
        public StatusMensalista Status { get; set; }
        public DayOfWeek Dia { get; set; }
        public TimeOnly Horario { get; set; }
        [SetsRequiredMembers]
        public MensalistaDetalhesResponse(Mensalista mensalista)
        {
            Id = mensalista.Id;
            Nome = mensalista.Nome;
            Celular = mensalista.Numero;
            Valor = mensalista.Valor;
            Tipo = mensalista.Tipo;
            Status = mensalista.Status;
            Dia = mensalista.Dia.DiaSemana;
            Horario = mensalista.Dia.Horario;
        }
    }
}
