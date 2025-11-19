using Api.Modelos.Request;
using Api.Modelos.Response;

namespace Api.Aplicacao.Contratos
{
    public interface IMensalistaApp
    {
        void CadastrarMensalista(MensalistaCriarRequest request, int idBarbeiro);
        void CancelarMensalista(int idMensalista);
        List<MensalistaDetalhesResponse> ListarMensalistas(int idBarbeiro);
    }
}
