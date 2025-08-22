using Api.Modelos.Dtos;
using Api.Modelos.Response;
using Api.Models.Entity;

namespace Api.Aplicacao.Contratos
{
    public interface IBarbeiroApp
    {
        void Cadastrar(BarbeiroCriarRequest barbeiro);
        void Editar(Barbeiro barbeiro);
        void Excluir(int id);
        BarbeiroDetalhesResponse BarbeiroDetalhes(int id);
        List<BarbeiroDetalhesResponse> ListaBarbeiros();
    }
}
