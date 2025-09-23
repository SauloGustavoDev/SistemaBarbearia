namespace Api.Modelos.Request
{
    public class BarbeiroEsqueceSenhaRequest
    {
        public required string Numero { get; set; }
        public required string Email { get; set; }
    }
}
