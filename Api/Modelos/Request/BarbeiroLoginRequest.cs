namespace Api.Modelos.Request
{
    public class BarbeiroLoginRequest
    {
        public required string Numero { get; set; }
        public required string Senha { get; set; }
    }
}
