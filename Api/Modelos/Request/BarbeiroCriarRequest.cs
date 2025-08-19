using Api.Modelos.Enums;

namespace Api.Modelos.Dtos
{
    public class BarbeiroCriarRequest
    {
        public string Nome { get; set; }
        public string Numero { get; set; }
        public string Email { get; set; }
        public Acesso Acesso { get; set; }
        public string Descricao { get; set; }
        public string Senha { get; set; }
        
    }
}
