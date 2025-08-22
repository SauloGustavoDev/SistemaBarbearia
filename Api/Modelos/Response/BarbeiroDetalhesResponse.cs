using Api.Modelos.Entidades;
using Api.Modelos.Enums;
using Api.Models.Entity;

namespace Api.Modelos.Response
{
    public class BarbeiroDetalhesResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public List<ServicosDetalhesResponse> Servicos { get; set; } = new List<ServicosDetalhesResponse>();

        public BarbeiroDetalhesResponse(Barbeiro barbeiro)
        {
            Id = barbeiro.Id;
            Nome = barbeiro.Nome;
            foreach (var item in barbeiro.Servicos)
            {
                Servicos.Add(new ServicosDetalhesResponse { Descricao = item.Descricao.ToString(), Valor = item.Valor });
            }
        }
    }
}
