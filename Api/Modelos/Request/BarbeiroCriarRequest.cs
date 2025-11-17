using Api.Modelos.Enums;

namespace Api.Modelos.Dtos
{
    public class BarbeiroCriarRequest
    {
        public required string Nome { get; set; }
        public required string Numero { get; set; }
        public required string Email { get; set; }
        public Acesso Acesso { get; set; }
        public TipoAgenda Agenda { get; set; } = TipoAgenda.Fechada;
        public required string Descricao { get; set; }
        public required string Senha { get; set; }
        
    }
}
