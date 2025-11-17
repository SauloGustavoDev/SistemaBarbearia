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

        public void Cadastrar(BarbeiroCriarRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Senha))
                throw new ArgumentException("A senha é obrigatória.");

            if (string.IsNullOrWhiteSpace(request.Numero))
                throw new ArgumentException("O numero é obrigatório");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("O email é obrigatório");

            if (string.IsNullOrWhiteSpace(request.Nome))
                throw new ArgumentException("O nome é obrigatório");

            if (request.Acesso == 0)
                throw new ArgumentException("O acesso é obrigatório");

            var numeroExiste = _contexto.Barbeiro.Any(b => (b.Numero == request.Numero || b.Email == request.Email) && b.DtDemissao == null);
            if(numeroExiste)
            throw new Exception("Telefone ou email já cadastrados.");

            request.Senha = Criptografia.GerarSenha(request.Senha);

            _contexto.Add(new Barbeiro(request));
            _contexto.SaveChanges();

        }

        public void Editar(BarbeiroEditarRequest request)
        {
            var barbeiro = _contexto.Barbeiro.Find(request.Id);
            if (barbeiro == null)
                throw new Exception("Barbeiro não encontrado");

            barbeiro.Nome = request.Nome;
            barbeiro.Email = request.Email;
            barbeiro.Numero = request.Numero;
            barbeiro.Descricao = request.Descricao;
            barbeiro.Agenda = request.Agenda;

            _contexto.SaveChanges();
        }

        public void Excluir(int id)
        {
            var barbeiro = _contexto.Barbeiro.Find(id) ?? throw new Exception("Barbeiro não encontrado");
            barbeiro.DtDemissao = DateTime.UtcNow;
            _contexto.SaveChanges();
        }

        public BarbeiroDetalhesResponse BarbeiroDetalhes(int id)
        {
            var barbeiro =  _contexto.Set<Barbeiro>()
                           .AsNoTracking()
                           .Include(x => x.BarbeiroServicos)
                           .Include(x => x.Agendamentos)
                           .Include(x => x.BarbeiroHorarios)
                           .FirstOrDefault(x => x.Id == id) ?? throw new Exception("Barbeiro não encontrado");

            return new BarbeiroDetalhesResponse(barbeiro);
        }

        public ResultadoPaginado<BarbeiroDetalhesResponse> ListaBarbeiros(PaginacaoFiltro request)
        {
            var barbeiros = _contexto.Set<Barbeiro>()
                .AsNoTracking()
                .Where(x => x.DtDemissao == null)
                .Select(x => new BarbeiroDetalhesResponse(x))
                .AsQueryable();
            return Paginacao.CriarPaginacao(barbeiros, request.Pagina, request.ItensPorPagina);
        }
    }
}
