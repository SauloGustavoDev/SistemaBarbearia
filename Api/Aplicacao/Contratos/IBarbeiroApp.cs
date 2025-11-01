using Api.Modelos.Dtos;
using Api.Modelos.Paginacao;
using Api.Modelos.Request;
using Api.Modelos.Response;

namespace Api.Aplicacao.Contratos
{
    public interface IBarbeiroApp
    {
        void Cadastrar(BarbeiroCriarRequest request);
        void Editar(BarbeiroEditarRequest request);
        void Excluir(int id);
        BarbeiroDetalhesResponse BarbeiroDetalhes(int id);
        ResultadoPaginado<BarbeiroDetalhesResponse> ListaBarbeiros(PaginacaoFiltro request);
    }
}
