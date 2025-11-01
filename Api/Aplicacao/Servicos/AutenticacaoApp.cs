using Api.Aplicacao.Contratos;
using Api.Aplicacao.Helpers;
using Api.Infraestrutura;
using Api.Infraestrutura.Contexto;
using Api.Modelos.Entidades;
using Api.Modelos.Request;
using Api.Modelos.Response;
using Microsoft.EntityFrameworkCore;

namespace Api.Aplicacao.Servicos
{
    public class AutenticacaoApp(Contexto contexto, INotificacaoApp notificacao, TokenProvider token) : IAutenticacaoApp
    {
        public readonly Contexto _contexto = contexto;
        private readonly INotificacaoApp _notificacao = notificacao;
        private readonly TokenProvider _token = token;

        public void AtualizarSenha(int id, string novaSenha)
        {
            var barbeiro =  _contexto.Set<Barbeiro>()
                            .Find(id) ?? throw new Exception("Barbeiro não encontrado");


            barbeiro.Senha = Criptografia.GerarSenha(novaSenha); // ⚠️ de preferência a senha deve estar hasheada
            _contexto.SaveChanges();
        }
        public void EsqueceuSenha(BarbeiroEsqueceSenhaRequest request)
        {
            var barbeiro = _contexto.Set<Barbeiro>()
                            .AsNoTracking()
                            .Where(x => x.Numero == request.Numero && x.Email == request.Email)
                            .FirstOrDefault() ?? throw new Exception("Barbeiro não encontrado");

            if (barbeiro.Email == null)
                throw new Exception("Email não encontrado");

            var token = _token.CreateToken(barbeiro);
            _notificacao.SendEmailNewPassword(barbeiro.Email, token);
        }

        public string Login(BarbeiroLoginRequest login)
        {
            var barbeiro = _contexto.Set<Barbeiro>()
                            .AsNoTracking()
                            .FirstOrDefault(x => x.Numero == login.Numero) ?? throw new Exception("Barbeiro não encontrado");

            if (Criptografia.VerificarSenha(login.Senha, barbeiro.Senha))
                return _token.CreateToken(barbeiro);
            else
                throw new Exception("Senha inválida");
        }
    }
}
