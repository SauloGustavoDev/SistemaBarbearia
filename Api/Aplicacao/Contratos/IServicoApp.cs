using Api.Modelos.Request;
using Api.Modelos.Response;

namespace Api.Aplicacao.Contratos
{
    public interface IServicoApp
    {
        List<ServicosDetalhesResponse> ListarServicosBarbeiro(int idBarbeiro);
        GenericResponse EditarServicosBarbeiro(ServicoBarbeiroEditarRequest request);
        GenericResponse CriarServico(ServicoCriarRequest request);
        GenericResponse DeletarServico(int id);
        GenericResponse AtualizarServico(ServicoAtualizarRequest request);
        List<ServicosDetalhesResponse> ListarServicos();
        GenericResponse CriarCategoriaServico(string request);
    }
}
