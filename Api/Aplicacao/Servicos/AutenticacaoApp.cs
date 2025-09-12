using Api.Aplicacao.Contratos;
using Api.Aplicacao.Helpers;
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

        public GenericResponse AtualizarSenha(int id, string novaSenha)
        {
            var barbeiro = _contexto.Set<Barbeiro>()
                            .FirstOrDefault(b => b.Id == id);

            if (barbeiro == null)
                return new GenericResponse { Sucesso = false, ErrorMessage = "Barbeiro não encontrado" };

            barbeiro.Senha = novaSenha; // ⚠️ de preferência a senha deve estar hasheada

            return MontarGenericResponse.TryExecute(() =>
            {
                _contexto.Update(barbeiro);
                _contexto.SaveChanges();
            }, "Falha ao atualizar a senha");
        }
        public GenericResponse EsqueceuSenha(BarbeiroEsqueceSenhaRequest request)
        {
            var barbeiro = _contexto.Set<Barbeiro>()
                            .AsNoTracking()
                            .Where(x => x.Numero == request.Numero && x.Email == request.Email)
                            .FirstOrDefault();

            if (barbeiro == null)
                return new GenericResponse { Sucesso = false, ErrorMessage = "Numero ou email invalido" };

            if (barbeiro.Email == null)
                return new GenericResponse { Sucesso = false, ErrorMessage = "Numero ou email invalido" };



            return MontarGenericResponse.TryExecute(() =>
            {
                var token = _token.CreateToken(barbeiro);
                _notificacao.SendEmailNewPassword(barbeiro.Email, token);
            }, "Falha ao atualizar a senha");
        }

        public GenericResponse Login(BarbeiroLoginRequest login)
        {
            var barbeiro = _contexto.Set<Barbeiro>()
                            .AsNoTracking()
                            .FirstOrDefault(x => x.Numero == login.Numero && x.Senha == login.Senha);

            if (barbeiro == null)
                return new GenericResponse { Sucesso = false, ErrorMessage = "Usuário ou senha incorretos" };

            return new GenericResponse { Sucesso = true, Token = _token.CreateToken(barbeiro) };
        }
    }
}
