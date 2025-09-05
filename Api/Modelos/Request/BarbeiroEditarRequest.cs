using Api.Modelos.Enums;

namespace Api.Modelos.Request
{
    public class BarbeiroEditarRequest
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Numero { get; set; }
        public string Descricao { get; set; }
        public byte[] Foto { get; set; }
        public Acesso Acesso { get; set; }
    }
}
