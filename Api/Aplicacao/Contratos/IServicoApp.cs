using Api.Modelos.Response;

namespace Api.Aplicacao.Contratos
{
    public interface IServicoApp
    {
        List<ServicosDetalhesResponse> ListarServicos(int idBarbeiro);
    }
}
