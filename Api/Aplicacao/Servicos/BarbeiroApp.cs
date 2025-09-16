using Api.Aplicacao.Contratos;
using Api.Aplicacao.Helpers;
using Api.Infraestrutura.Contexto;
using Api.Modelos.Dtos;
using Api.Modelos.Entidades;
using Api.Modelos.Request;
using Api.Modelos.Response;
using Api.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace Api.Aplicacao.Servicos
{
    public class BarbeiroApp : IBarbeiroApp
    {
        public readonly Contexto _contexto;
        public BarbeiroApp(Contexto contexto)
        {
            _contexto = contexto;
        }
        public GenericResponse Cadastrar(BarbeiroCriarRequest request)
        {
            var erros = new List<string>();
            if (string.IsNullOrWhiteSpace(request.Senha))
                erros.Add("A senha é obrigatória.");

            if (string.IsNullOrWhiteSpace(request.Numero))
                erros.Add("O numero é obrigatório");

            if (string.IsNullOrWhiteSpace(request.Email))
                erros.Add("O email é obrigatório");

            if (string.IsNullOrWhiteSpace(request.Nome))
                erros.Add("O nome é obrigatório");

            if (request.Acesso == 0)
                erros.Add("O acesso é obrigatório");

            if (erros.Any())
                return new GenericResponse { Sucesso = false, ErrorMessage = string.Join(" ", erros) };

            request.Senha = Criptografia.GerarSenha(request.Senha);

            return MontarGenericResponse.TryExecute(() =>
            {
                _contexto.Add(new Barbeiro(request));
                _contexto.SaveChanges();
            }, "Falha ao cadastrar barbeiro.");

        }

        public GenericResponse Editar(BarbeiroEditarRequest request)
        {
            var barbeiro = _contexto.Barbeiro.FirstOrDefault(b => b.Id == request.Id);
            if (barbeiro == null)
                return new GenericResponse { Sucesso = false, ErrorMessage = "Barbeiro não encontrado" };

            barbeiro.Nome = request.Nome;
            barbeiro.Email = request.Email;
            barbeiro.Numero = request.Numero;
            barbeiro.Descricao = request.Descricao;

            return MontarGenericResponse.TryExecute(() =>
            {
                _contexto.SaveChanges();
            }, "Falha ao cadastrar barbeiro.");
        }

        public GenericResponse Excluir(int id)
        {

            var barbeiro = _contexto.Barbeiro.FirstOrDefault(b => b.Id == id);
            if (barbeiro == null)
                return new GenericResponse { Sucesso = false, ErrorMessage = "Barbeiro não encontrado" };

            barbeiro.DtDemissao = DateTime.UtcNow;

            return MontarGenericResponse.TryExecute(() =>
            {
                _contexto.SaveChanges();
            }, "Falha ao remover barbeiro.");
        }

        public BarbeiroDetalhesResponse BarbeiroDetalhes(int id)
        {
            var barbeiro = _contexto.Set<Barbeiro>()
                           .AsNoTracking()
                           .Include(x => x.BarbeiroServicos)
                           .Include(x => x.Agendamentos)
                           .Include(x => x.BarbeiroHorarios)
                           .FirstOrDefault(x => x.Id == id) ?? throw new Exception("Barbeiro não encontrado");

            return new BarbeiroDetalhesResponse(barbeiro);
        }

        public List<BarbeiroDetalhesResponse> ListaBarbeiros()
        {
            var barbeiros = _contexto.Set<Barbeiro>()
                .AsNoTracking()
                .Where(x => x.DtDemissao == null)
                .ToList();

            var data = new List<BarbeiroDetalhesResponse>();
            foreach (var barbeiro in barbeiros)
            {
                data.Add(new BarbeiroDetalhesResponse(barbeiro));
            }

            return data;
        }
    }
}
