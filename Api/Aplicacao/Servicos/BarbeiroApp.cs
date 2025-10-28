using Api.Aplicacao.Contratos;
using Api.Aplicacao.Helpers;
using Api.Infraestrutura.Contexto;
using Api.Modelos.Dtos;
using Api.Modelos.Entidades;
using Api.Modelos.Paginacao;
using Api.Modelos.Request;
using Api.Modelos.Response;
using Microsoft.EntityFrameworkCore;

namespace Api.Aplicacao.Servicos
{
    public class BarbeiroApp(Contexto contexto) : IBarbeiroApp
    {
        public readonly Contexto _contexto = contexto;

        public async Task<GenericResponse> Cadastrar(BarbeiroCriarRequest request)
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

            if (erros.Count != 0)
                return new GenericResponse { Sucesso = false, ErrorMessage = string.Join(" ", erros) };

            request.Senha = Criptografia.GerarSenha(request.Senha);

            return await MontarGenericResponse.TryExecuteAsync(async () =>
            {
                await _contexto.AddAsync(new Barbeiro(request));
                await _contexto.SaveChangesAsync();
            }, "Falha ao cadastrar barbeiro.");

        }

        public async Task<GenericResponse> Editar(BarbeiroEditarRequest request)
        {
            var barbeiro = await _contexto.Barbeiro.FirstOrDefaultAsync(b => b.Id == request.Id);
            if (barbeiro == null)
                return new GenericResponse { Sucesso = false, ErrorMessage = "Barbeiro não encontrado" };

            barbeiro.Nome = request.Nome;
            barbeiro.Email = request.Email;
            barbeiro.Numero = request.Numero;
            barbeiro.Descricao = request.Descricao;
            barbeiro.Agenda = request.Agenda;

            return await MontarGenericResponse.TryExecuteAsync(async() =>
            {
                await _contexto.SaveChangesAsync();
            }, "Falha ao cadastrar barbeiro.");
        }

        public async Task<GenericResponse> Excluir(int id)
        {
            var barbeiro = await _contexto.Barbeiro.FirstOrDefaultAsync(b => b.Id == id);
            if (barbeiro == null)
                return new GenericResponse { Sucesso = false, ErrorMessage = "Barbeiro não encontrado" };

            barbeiro.DtDemissao = DateTime.UtcNow;

            return await MontarGenericResponse.TryExecuteAsync(async () =>
            {
                await _contexto.SaveChangesAsync();
            }, "Falha ao remover barbeiro.");
        }

        public async Task<BarbeiroDetalhesResponse> BarbeiroDetalhes(int id)
        {
            var barbeiro = await _contexto.Set<Barbeiro>()
                           .AsNoTracking()
                           .Include(x => x.BarbeiroServicos)
                           .Include(x => x.Agendamentos)
                           .Include(x => x.BarbeiroHorarios)
                           .FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception("Barbeiro não encontrado");

            return new BarbeiroDetalhesResponse(barbeiro);
        }

        public async Task<ResultadoPaginado<BarbeiroDetalhesResponse>> ListaBarbeiros(PaginacaoFiltro request)
        {
            var barbeiros = _contexto.Set<Barbeiro>()
                .AsNoTracking()
                .Where(x => x.DtDemissao == null)
                .Select(x => new BarbeiroDetalhesResponse(x))
                .AsQueryable();



            return await Paginacao.CriarPaginacao(barbeiros, request.Pagina, request.ItensPorPagina);
        }
    }
}
