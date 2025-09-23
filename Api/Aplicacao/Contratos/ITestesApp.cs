using Api.Modelos.Response;

namespace Api.Aplicacao.Contratos
{
    public interface ITestesApp
    {
        Task<GenericResponse> GerarBancoSimulado();
        Task<GenericResponse> LimparBancoDeDados();
    }
}
