namespace Api.Modelos.Entidades
{
    public class CodigoConfirmacao
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public int Codigo { get; set; }
        public DateTime DtCriacao { get; set; }
        public DateTime DtExpiracao { get; set; }
        public bool Confirmado { get; set; } = false;
        public bool Reenviado { get; set; } = false;
        public CodigoConfirmacao(string numero, int codigo)
        {
            Numero = numero;
            Codigo = codigo;
            DtCriacao = DateTime.UtcNow;
            DtExpiracao = DtCriacao.AddMinutes(10);
        }
    }
}
