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

        public async Task<GenericResponse> AtualizarSenha(int id, string novaSenha)
        {
            var barbeiro = await _contexto.Set<Barbeiro>()
                            .FirstOrDefaultAsync(b => b.Id == id);

            if (barbeiro == null)
                return new GenericResponse { Sucesso = false, ErrorMessage = "Barbeiro não encontrado" };

            barbeiro.Senha = Criptografia.GerarSenha(novaSenha); // ⚠️ de preferência a senha deve estar hasheada

            return await MontarGenericResponse.TryExecuteAsync(async () =>
            {
                await _contexto.SaveChangesAsync();
            }, "Falha ao atualizar a senha");
        }
        public async Task<GenericResponse> EsqueceuSenha(BarbeiroEsqueceSenhaRequest request)
        {
            var barbeiro = await _contexto.Set<Barbeiro>()
                            .AsNoTracking()
                            .Where(x => x.Numero == request.Numero && x.Email == request.Email)
                            .FirstOrDefaultAsync();

            if (barbeiro == null)
                return new GenericResponse { Sucesso = false, ErrorMessage = "Numero ou email invalido" };

            if (barbeiro.Email == null)
                return new GenericResponse { Sucesso = false, ErrorMessage = "Numero ou email invalido" };

            return await MontarGenericResponse.TryExecuteAsync(async () =>
            {
                var token = _token.CreateToken(barbeiro);
                _notificacao.SendEmailNewPassword(barbeiro.Email, token);
                await Task.CompletedTask;
            }, "Falha ao atualizar a senha");
        }

        public GenericResponse Login(BarbeiroLoginRequest login)
        {
            var barbeiro = _contexto.Set<Barbeiro>()
                            .AsNoTracking()
                            .FirstOrDefault(x => x.Numero == login.Numero);


            if (barbeiro == null)
                return new GenericResponse { Sucesso = false, ErrorMessage = "Usuário ou senha incorretos" };

            if (Criptografia.VerificarSenha(login.Senha, barbeiro.Senha))
                return new GenericResponse { Sucesso = true, Token = _token.CreateToken(barbeiro) };
            else
                return new GenericResponse { Sucesso = false, ErrorMessage = "Usuário ou senha incorretos" };
        }
    }
}
