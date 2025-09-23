using Api.Modelos.Paginacao;
using Api.Modelos.Request;
using Api.Modelos.Response;

namespace Api.Aplicacao.Contratos
{
    public interface IServicoApp
    {
        Task<ResultadoPaginado<ServicosDetalhesResponse>> ListarServicosBarbeiro(int idBarbeiro, PaginacaoFiltro request);
        Task<GenericResponse> EditarServicosBarbeiro(ServicoBarbeiroEditarRequest request);
        Task<GenericResponse> CriarServico(ServicoCriarRequest request);
        Task<GenericResponse> DeletarServico(int id);
        Task<GenericResponse> AtualizarServico(ServicoAtualizarRequest request);
        Task<ResultadoPaginado<ServicosDetalhesResponse>> ListarServicos(PaginacaoFiltro request);
        Task<GenericResponse> CriarCategoriaServico(string request);
    }
}
