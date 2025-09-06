using Api.Modelos.Response;

namespace Api.Aplicacao.Contratos
{
    public interface ITestesApp
    {
        GenericResponse GerarBancoSimulado();
        GenericResponse LimparBancoDeDados();
    }
}
