using Api.Aplicacao.Contratos;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;

namespace Api.Aplicacao.Servicos
{
    public class NotificacaoApp(IConfiguration configuration) : INotificacaoApp
    {
        private readonly IConfiguration _config = configuration;

        public void SendEmailNewPassword(string email, string token)
        {
            var fromAddress = _config.GetSection("Email:From").Value;
            if (string.IsNullOrWhiteSpace(fromAddress))
                throw new InvalidOperationException("O endereço de e-mail de origem não está configurado.");

            var mail = new MailMessage
            {
                Subject = "GS System - Redefinição de Senha",
                From = new MailAddress(fromAddress, "GS System")
            };
            mail.To.Add(email);

            // --- TEMPLATE HTML ---
            mail.Body = $@"
        <!DOCTYPE html>
        <html lang=""pt-BR"">
        <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <style>
                body {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; }}
                .container {{ max-width: 600px; margin: 40px auto; background-color: #ffffff; border: 1px solid #dddddd; border-radius: 8px; overflow: hidden; }}
                .header {{ background-color: #007bff; color: #ffffff; padding: 20px; text-align: center; }}
                .content {{ padding: 30px; line-height: 1.6; color: #333333; }}
                .content h1 {{ color: #007bff; font-size: 24px; }}
                .button {{ display: inline-block; background-color: #007bff; color: #ffffff; padding: 12px 25px; text-align: center; text-decoration: none; border-radius: 5px; font-size: 16px; margin-top: 20px; }}
                .footer {{ background-color: #f4f4f4; color: #888888; padding: 20px; text-align: center; font-size: 12px; }}
            </style>
        </head>
        <body>
            <div class=""container"">
                <div class=""header"">
                    <h2>GS System</h2>
                </div>
                <div class=""content"">
                    <h1>Redefinição de Senha</h1>
                    <p>Olá,</p>
                    <p>Recebemos uma solicitação para redefinir a senha da sua conta. Para continuar, clique no botão abaixo:</p>
                    <a href='{_config.GetSection("Proprietario:Dominio").Value}/Nova-Senha?token={token}' class=""button"">Redefinir Minha Senha</a>
                    <p>Se você não solicitou a alteração de senha, por favor, ignore este e-mail.</p>
                    <p>O link é válido por 12 horas.</p>
                      

                    <p>Atenciosamente,  
        Equipe GS System</p>
                </div>
                <div class=""footer"">
                    <p>&copy; {DateTime.Now.Year} GS System. Todos os direitos reservados.</p>
                </div>
            </div>
        </body>
        </html>
        ";
            mail.IsBodyHtml = true;

            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;

            var host = _config.GetSection("Email:Host").Value;
            if (string.IsNullOrWhiteSpace(host))
                throw new InvalidOperationException("O endereço de e-mail de origem não está configurado.");

            using var smtp = new SmtpClient();
            smtp.Host = host;
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(
                _config.GetSection("Email:Username").Value,
                _config.GetSection("Email:Password").Value
            );

            smtp.Send(mail);
        }
    }
}
