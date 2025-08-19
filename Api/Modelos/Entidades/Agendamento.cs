using Api.Modelos.Enums;
using Api.Models.Entity;

namespace Api.Modelos.Entidades
{
    public class Agendamento
    {
        public int Id { get; set; }
        public Barbeiro Barbeiro { get; set; }
        public List<AgendamentoHorario> AgendamentoHorario{ get; set; }
        public List<Servico> Servico { get; set; }
        public Status Status { get; set; }
        public string Cliente { get; set; }
        public string NumeroCliente { get; set; }
    }
}
