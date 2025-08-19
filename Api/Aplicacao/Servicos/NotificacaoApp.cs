using Api.Aplicacao.Contratos;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;

namespace Api.Aplicacao.Servicos
{
    public class NotificacaoApp : INotificacaoApp
    {
        private readonly IConfiguration _config;
        public NotificacaoApp(IConfiguration configuration)
        {
            _config = configuration;
        }
        public void SendEmailNewPassword(string email, string token)
        {
            var mail = new MailMessage();
            mail.Subject = "GS System - Nova Senha"; 
            mail.Body = $@"Olá, segue abaixo o link para redefinição de senha!
                           <br><a href='{_config.GetSection("Proprietario:Dominio").Value}/Nova-Senha?token={token}";
            mail.From = new MailAddress(_config.GetSection("Email:From").Value, "NoReply");
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;

            new SmtpClient()
            {
                Host = _config.GetSection("Email:Host").Value,
                Port = 587,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(_config.GetSection("Email:Username").Value, _config.GetSection("Email:Password").Value)
            }.Send(mail);
        }
    }
}
