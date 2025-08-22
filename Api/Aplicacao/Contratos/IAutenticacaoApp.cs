using Api.Modelos.Request;
using Api.Modelos.Response;

namespace Api.Aplicacao.Contratos
{
    public interface IAutenticacaoApp
    {
        GenericResponse Login(BarbeiroLoginRequest login);
        GenericResponse EsqueceuSenha(BarbeiroEsqueceSenhaRequest request);
        GenericResponse AtualizarSenha(int id, string novaSenha);
    }

}
