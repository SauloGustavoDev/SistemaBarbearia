namespace Api.Aplicacao.Contratos
{
    public interface INotificacaoApp
    {
        void SendEmailNewPassword(string email, string token);
    }
}
