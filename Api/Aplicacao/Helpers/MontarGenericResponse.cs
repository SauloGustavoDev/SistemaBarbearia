using Api.Modelos.Response;

namespace Api.Aplicacao.Helpers
{
    public static class MontarGenericResponse
    {
        public static async Task<GenericResponse> TryExecuteAsync(Func<Task> action, string errorMessage)
        {
            try
            {
                await action();
                return new GenericResponse { Sucesso = true };
            }
            catch
            {
                return new GenericResponse { Sucesso = false, ErrorMessage = errorMessage };
            }
        }

    }
}
