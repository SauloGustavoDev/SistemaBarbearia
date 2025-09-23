using Api.Modelos.Dtos;
using Api.Modelos.Paginacao;
using Api.Modelos.Request;
using Api.Modelos.Response;

namespace Api.Aplicacao.Contratos
{
    public interface IBarbeiroApp
    {
        Task<GenericResponse> Cadastrar(BarbeiroCriarRequest request);
        Task<GenericResponse> Editar(BarbeiroEditarRequest request);
        Task<GenericResponse> Excluir(int id);
        Task<BarbeiroDetalhesResponse> BarbeiroDetalhes(int id);
        Task<ResultadoPaginado<BarbeiroDetalhesResponse>> ListaBarbeiros(PaginacaoFiltro request);
    }
}
