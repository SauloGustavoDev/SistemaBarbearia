using Api.Modelos.Entidades;

namespace Api.Models.Entity
{
    public class Barbeiro
    {
        private int Id { get; set; }
        private string Nome { get; set; }
        private string Celular { get; set; }
        private List<Servico> Servicos { get; set; }
        private List<Horario> Horarios { get; set; }
    }
}
