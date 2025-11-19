using Api.Aplicacao.Contratos;
using Api.Aplicacao.Helpers;
using Api.Infraestrutura.Contexto;
using Api.Modelos.Entidades;
using Api.Modelos.Paginacao;
using Api.Modelos.Request;
using Api.Modelos.Response;
using Microsoft.EntityFrameworkCore;

namespace Api.Aplicacao.Servicos
{
    public class ServicoApp(Contexto contexto) : IServicoApp
    {
        public readonly Contexto _contexto = contexto;

        public void CriarServico(ServicoCriarRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Descricao))
                throw new ArgumentException("A descrição do serviço é obrigatória.");

            if (request.Valor <= 0)
                throw new ArgumentException("O valor do serviço deve ser maior que zero.");

            if (request.TempoEstimado == 0)
                throw new ArgumentException("O tempo estimado do serviço deve ser maior que zero.");

            if (request.Categoria <= 0)
                throw new ArgumentException("A categoria do serviço é obrigatória.");

            var categoriaExiste = _contexto.CategoriaServico.Any(c => c.Id == request.Categoria);

            if (!categoriaExiste)
                throw new ArgumentException($"A categoria com ID {request.Categoria} não foi encontrada.");

            var novoServico = new Servico
            {
                Descricao = request.Descricao,
                Valor = request.Valor,
                TempoEstimado = new TimeOnly(0, request.TempoEstimado, 0),
                DtInicio = DateTime.UtcNow,
                IdCategoriaServico = request.Categoria
            };
            _contexto.Servico.Add(novoServico);
            _contexto.SaveChanges();
        }
        public void AtualizarServico(ServicoAtualizarRequest request)
        {
            var servicoAtual = _contexto.Servico.FirstOrDefault(s => s.Id == request.Id && s.DtFim == null) ?? throw new ArgumentException($"Serviço ativo com ID {request.Id} não foi encontrado.");

            var categoriaExiste = _contexto.CategoriaServico.FirstOrDefault(c => c.Id == request.Categoria) ?? throw new ArgumentException($"A categoria com ID {request.Categoria} não foi encontrada.");

            servicoAtual.DtFim = DateTime.UtcNow;

            var novoServico = new Servico
            {
                Descricao = request.Descricao!,
                Valor = request.Valor,
                TempoEstimado = new TimeOnly(0, request.TempoEstimado, 0),
                DtInicio = DateTime.UtcNow,
                IdCategoriaServico = request.Categoria
            };

            _contexto.Servico.Add(novoServico);
            _contexto.SaveChanges();
        }

        public void CriarCategoriaServico(string request)
        {
            var novaCategoria = new CategoriaServico
            {
                Descricao = request,
                DtInicio = DateTime.UtcNow
            };

            _contexto.CategoriaServico.Add(novaCategoria);
            _contexto.SaveChanges();
        }

        public void DeletarServico(int id)
        {
            var servicoParaDesativar = _contexto.Servico.FirstOrDefault(s => s.Id == id && s.DtFim == null) ?? throw new ArgumentException($"Serviço ativo com ID {id} não foi encontrado.");
            servicoParaDesativar.DtFim = DateTime.UtcNow;
            _contexto.SaveChanges();
        }
        public void EditarServicosBarbeiro(ServicoBarbeiroEditarRequest request)
        {
            using var transaction = _contexto.Database.BeginTransaction();
            try
            {
                var barbeiroExiste = _contexto.Barbeiro.Any(b => b.Id == request.IdBarbeiro);

                if (!barbeiroExiste)
                    throw new Exception($"Barbeiro com ID {request.IdBarbeiro} não existe.");

                var servicosAtuaisAtivos =  _contexto.BarbeiroServico
                    .Where(bs => bs.IdBarbeiro == request.IdBarbeiro && bs.DtFim == null)
                    .ToList();

                var idsServicosAtuais = servicosAtuaisAtivos.Select(bs => bs.IdServico).ToList();
                var idsParaAdicionar = request.IdsServico.Except(idsServicosAtuais).ToList();

                var servicosParaDesativar = servicosAtuaisAtivos
                    .Where(bs => !request.IdsServico.Contains(bs.IdServico))
                    .ToList();

                foreach (var servico in servicosParaDesativar)
                {
                    servico.DtFim = DateTime.UtcNow;
                }

                foreach (var idServico in idsParaAdicionar)
                {
                    var servicoExiste =  _contexto.Servico.Any(s => s.Id == idServico);
                    if (!servicoExiste)
                    {
                        transaction.Rollback();
                       throw new Exception($"Serviço com ID {idServico} não existe.");
                    }

                    var novoServicoBarbeiro = new BarbeiroServico
                    {
                        IdBarbeiro = request.IdBarbeiro,
                        IdServico = idServico,
                        DtInicio = DateTime.UtcNow,
                        DtFim = null
                    };
                     _contexto.BarbeiroServico.Add(novoServicoBarbeiro);
                }

                 _contexto.SaveChanges();
                 transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw new Exception("Ocorreu um erro ao editar os serviços do barbeiro.");
            }
        }

        public ResultadoPaginado<ServicosDetalhesResponse> ListarServicosBarbeiro(int idBarbeiro, PaginacaoFiltro request)
        {
            var query = _contexto.BarbeiroServico
                .Where(s => s.IdBarbeiro == idBarbeiro && s.DtFim == null)
                .Include(x => x.Servico)
                .ThenInclude(x => x!.CategoriaServico)
                .OrderBy(x => x.Servico!.Descricao)
                .Select(s => new ServicosDetalhesResponse
                {
                    Id = s.Id,
                    Descricao = s.Servico!.Descricao,
                    Valor = s.Servico.Valor,
                    Categoria = s.Servico.CategoriaServico!.Descricao!,
                    TempoEstimado = s.Servico.TempoEstimado
                })
                .AsQueryable();

            return Paginacao.CriarPaginacao(query, request.Pagina, request.ItensPorPagina);
        }

        public ResultadoPaginado<ServicosDetalhesResponse> ListarServicos(PaginacaoFiltro request)
        {
            var query = _contexto.Servico
                .Where(s => s.DtFim == null)
                .Include(x => x.CategoriaServico)
                .OrderBy(x => x.Descricao)
                .Select(s => new ServicosDetalhesResponse
                {
                    Id = s.Id,
                    Descricao = s.Descricao,
                    Valor = s.Valor,
                    TempoEstimado = s.TempoEstimado,
                    Categoria = s.CategoriaServico!.Descricao!
                })
                .AsQueryable();

            return Paginacao.CriarPaginacao(query, request.Pagina, request.ItensPorPagina);
        }

        public List<CategoriasResponse> ListarCategorias()
        {
            var categorias = _contexto.CategoriaServico
                .Where(c => c.DtFim == null)
                .Include(c => c.Servicos)
                .Select(c => new CategoriasResponse
                {
                    Id = c.Id,
                    Descricao = c.Descricao
                })
                .ToList();
            return categorias;
        }
    }
}
