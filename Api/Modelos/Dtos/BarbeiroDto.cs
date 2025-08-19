using Api.Modelos.Enums;

namespace Api.Modelos.Dtos
{
    public class BarbeiroDto
    {
        public string Nome { get; set; }
        public string Numero { get; set; }
        public Acesso Acesso { get; set; }
        public string Descricao { get; set; }
        public string Senha { get; set; }
    }
}
