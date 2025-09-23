using Api.Modelos.Entidades;

namespace Api.Modelos.Response
{
    public class BarbeiroDetalhesResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Numero { get; set; }
        public string Email { get; set; }
        public byte[]? Foto { get; set; }
        public string? Descricao { get; set; }
        public List<ServicosDetalhesResponse> Servicos { get; set; } = [];

        public BarbeiroDetalhesResponse(Barbeiro barbeiro)
        {
            Id = barbeiro.Id;
            Nome = barbeiro.Nome;
            Numero = barbeiro.Numero;
            Email = barbeiro.Email;
            Foto = barbeiro.Foto;
            Descricao = barbeiro.Descricao;
            if (barbeiro.BarbeiroServicos != null)
            {
                foreach (var item in barbeiro.BarbeiroServicos)
                {
                    Servicos.Add(new ServicosDetalhesResponse
                    {
                        Descricao = item.Servico!.Descricao.ToString(),
                        Valor = item.Servico.Valor
                    });
                }
            }
        }
    }
}
