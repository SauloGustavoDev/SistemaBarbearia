using Api.Modelos.Dtos;
using Api.Modelos.Request;
using Api.Modelos.Response;
using Api.Models.Entity;

namespace Api.Aplicacao.Contratos
{
    public interface IBarbeiroApp
    {
        GenericResponse Cadastrar(BarbeiroCriarRequest request);
        GenericResponse Editar(BarbeiroEditarRequest request);
        GenericResponse Excluir(int id);
        BarbeiroDetalhesResponse BarbeiroDetalhes(int id);
        List<BarbeiroDetalhesResponse> ListaBarbeiros();
    }
}
