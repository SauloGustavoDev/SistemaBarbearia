using Api.Modelos.Paginacao;
using Api.Modelos.Request;
using Api.Modelos.Response;

namespace Api.Aplicacao.Contratos
{
    public interface IServicoApp
    {
        ResultadoPaginado<ServicosDetalhesResponse> ListarServicosBarbeiro(int idBarbeiro, PaginacaoFiltro request);
        void EditarServicosBarbeiro(ServicoBarbeiroEditarRequest request);
        void CriarServico(ServicoCriarRequest request);
        void DeletarServico(int id);
        void AtualizarServico(ServicoAtualizarRequest request);
        ResultadoPaginado<ServicosDetalhesResponse> ListarServicos(PaginacaoFiltro request);
        void CriarCategoriaServico(string request);
    }
}
