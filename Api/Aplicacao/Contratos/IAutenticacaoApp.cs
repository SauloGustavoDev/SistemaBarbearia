using Api.Modelos.Request;
using Api.Modelos.Response;

namespace Api.Aplicacao.Contratos
{
    public interface IAutenticacaoApp
    {
        GenericRespose Login(BarbeiroLoginRequest login);
        GenericRespose EsqueceuSenha(BarbeiroEsqueceSenhaRequest request);
    }
}
