using Api.Modelos.Response;

namespace Api.Aplicacao.Contratos
{
    public interface ITestesApp
    {
        void GerarBancoSimulado();
        void LimparBancoDeDados();
    }
}
