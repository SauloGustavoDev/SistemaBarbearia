using Api.Modelos.Request;
using Api.Modelos.Response;

namespace Api.Aplicacao.Contratos
{
    public interface IAutenticacaoApp
    {
        string Login(BarbeiroLoginRequest login);
        void EsqueceuSenha(BarbeiroEsqueceSenhaRequest request);
        void AtualizarSenha(int id, string novaSenha);
    }

}
