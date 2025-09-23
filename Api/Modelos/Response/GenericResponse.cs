namespace Api.Modelos.Response
{
    public class GenericResponse
    {
        public bool Sucesso { get; set; }
        public string? Token { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
