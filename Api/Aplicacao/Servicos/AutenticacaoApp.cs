using Api.Aplicacao.Contratos;
using Api.Infraestrutura;
using Api.Infraestrutura.Contexto;
using Api.Modelos.Request;
using Api.Modelos.Response;
using Api.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace Api.Aplicacao.Servicos
{
    public class AutenticacaoApp : IAutenticacaoApp
    {
        public readonly Contexto _contexto;
        private readonly INotificacaoApp _notificacao;
        private readonly TokenProvider _token;
        public AutenticacaoApp(Contexto contexto, INotificacaoApp notificacao, TokenProvider token)
        {
            _contexto = contexto;
            _notificacao = notificacao;
            _token = token;
        }

        public GenericRespose EsqueceuSenha(BarbeiroEsqueceSenhaRequest request)
        {
            var barbeiro = _contexto.Set<Barbeiro>()
                            .AsNoTracking()
                            .Where(x => x.Numero == request.Numero && x.Email == request.Email)
                            .FirstOrDefault();

            if (barbeiro.Email == null)
                return new GenericRespose { Successo = false, ErrorMessage = "Numero ou email invalido" };

            var token = _token.CreateToken(barbeiro);

            _notificacao.SendEmailNewPassword(barbeiro.Email, token);

            return new GenericRespose { Successo = true };
        }

        public GenericRespose Login(BarbeiroLoginRequest login)
        {
            var barbeiro = _contexto.Set<Barbeiro>()
                            .AsNoTracking()
                            .FirstOrDefault(x => x.Numero == login.Numero && x.Senha == login.Senha);

            if(barbeiro == null)
                return new GenericRespose{
                    Successo = false, 
                    ErrorMessage = "Usuário ou senha incorretos" 
                };

            return new GenericRespose
            {
                Successo = true,
                Token = _token.CreateToken(barbeiro)
            };
        }
    }
}
