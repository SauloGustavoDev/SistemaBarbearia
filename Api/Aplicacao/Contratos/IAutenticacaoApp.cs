using Api.Modelos.Request;
using Api.Modelos.Response;

namespace Api.Aplicacao.Contratos
{
    public interface IAutenticacaoApp
    {
        GenericResponse Login(BarbeiroLoginRequest login);
        Task<GenericResponse> EsqueceuSenha(BarbeiroEsqueceSenhaRequest request);
        Task<GenericResponse> AtualizarSenha(int id, string novaSenha);
    }

}
