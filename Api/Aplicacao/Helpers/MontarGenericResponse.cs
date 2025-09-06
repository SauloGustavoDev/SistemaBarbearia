using Api.Modelos.Response;

namespace Api.Aplicacao.Helpers
{
    public static class MontarGenericResponse
    {
        public static GenericResponse TryExecute(Action action, string errorMessage)
        {
            try
            {
                action();
                return new GenericResponse { Sucesso = true };
            }
            catch
            {
                return new GenericResponse { Sucesso = false, ErrorMessage = errorMessage };
            }
        }
    }
}
