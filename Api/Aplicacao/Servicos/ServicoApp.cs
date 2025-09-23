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

        public async Task<GenericResponse> CriarServico(ServicoCriarRequest request)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Descricao))
                erros.Add("A descrição do serviço é obrigatória.");

            if (request.Valor <= 0)
                erros.Add("O valor do serviço deve ser maior que zero.");

            if (request.TempoEstimado == 0)
                erros.Add("O tempo estimado do serviço é obrigatório.");

            if (request.Categoria <= 0)
                erros.Add("A categoria do serviço é obrigatória.");

            if (erros.Count != 0)
                return new GenericResponse { Sucesso = false, ErrorMessage = string.Join(" ", erros) };

            var categoriaExiste = await _contexto.CategoriaServico.AnyAsync(c => c.Id == request.Categoria);
            if (!categoriaExiste)
                return new GenericResponse { Sucesso = false, ErrorMessage = $"A categoria com ID {request.Categoria} não foi encontrada." };

            var novoServico = new Servico
            {
                Descricao = request.Descricao,
                Valor = request.Valor,
                TempoEstimado = new TimeOnly(0, request.TempoEstimado, 0),
                DtInicio = DateTime.UtcNow,
                IdCategoriaServico = request.Categoria
            };

            return await MontarGenericResponse.TryExecuteAsync( async () =>
            {
                await _contexto.Servico.AddAsync(novoServico);
                await _contexto.SaveChangesAsync();
            }, "Ocorreu um erro inesperado ao criar o serviço.");
        }
        public async Task<GenericResponse> AtualizarServico(ServicoAtualizarRequest request)
        {
            var servicoAtual = await _contexto.Servico.FirstOrDefaultAsync(s => s.Id == request.Id && s.DtFim == null);

            if (servicoAtual == null)
                return new GenericResponse { Sucesso = false, ErrorMessage = $"Serviço ativo com ID {request.Id} não foi encontrado." };

            var categoriaExiste = _contexto.CategoriaServico.Any(c => c.Id == request.Categoria);
            if (!categoriaExiste)
                return new GenericResponse { Sucesso = false, ErrorMessage = $"A categoria com ID {request.Categoria} não foi encontrada." };

            servicoAtual.DtFim = DateTime.UtcNow;

            var novoServico = new Servico
            {
                Descricao = request.Descricao!,
                Valor = request.Valor,
                TempoEstimado = new TimeOnly(0,request.TempoEstimado,0),
                DtInicio = DateTime.UtcNow,
                IdCategoriaServico = request.Categoria
            };

            return await MontarGenericResponse.TryExecuteAsync(async () =>
            {
                await _contexto.Servico.AddAsync(novoServico);
                await _contexto.SaveChangesAsync();
            }, "Ocorreu um erro inesperado ao atualizar o serviço.");
        }

        public async Task<GenericResponse> CriarCategoriaServico(string request)
        {
            var novaCategoria = new CategoriaServico
            {
                Descricao = request,
                DtInicio = DateTime.UtcNow
            };
            return await MontarGenericResponse.TryExecuteAsync(async () =>
            {
                await _contexto.CategoriaServico.AddAsync(novaCategoria);
                await _contexto.SaveChangesAsync();
            }, "Ocorreu um erro inesperado ao criar a categoria.");
        }

        public async Task<GenericResponse> DeletarServico(int id)
        {
                var servicoParaDesativar = await _contexto.Servico.FirstOrDefaultAsync(s => s.Id == id && s.DtFim == null);
                if (servicoParaDesativar == null)
                    return new GenericResponse { Sucesso = false, ErrorMessage = $"Serviço ativo com ID {id} não foi encontrado." };

                servicoParaDesativar.DtFim = DateTime.UtcNow;
            return await MontarGenericResponse.TryExecuteAsync(async () =>
            {
                await _contexto.SaveChangesAsync();
            }, "Ocorreu um erro inesperado ao desativar o serviço.");
        }
        public async Task<GenericResponse> EditarServicosBarbeiro(ServicoBarbeiroEditarRequest request)
        {
            await using var transaction = await _contexto.Database.BeginTransactionAsync();
            try
            {
                var barbeiroExiste = await _contexto.Barbeiro.AnyAsync(b => b.Id == request.IdBarbeiro);

                if (!barbeiroExiste)
                    return new GenericResponse { Sucesso = false, ErrorMessage = "Barbeiro não encontrado." };


                var servicosAtuaisAtivos = await _contexto.BarbeiroServico
                    .Where(bs => bs.IdBarbeiro == request.IdBarbeiro && bs.DtFim == null)
                    .ToListAsync();

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
                    var servicoExiste = await _contexto.Servico.AnyAsync(s => s.Id == idServico);
                    if (!servicoExiste)
                    {
                        await transaction.RollbackAsync();
                        return new GenericResponse { Sucesso = false, ErrorMessage = $"Serviço com ID {idServico} não existe." };
                    }

                    var novoServicoBarbeiro = new BarbeiroServico
                    {
                        IdBarbeiro = request.IdBarbeiro,
                        IdServico = idServico,
                        DtInicio = DateTime.UtcNow,
                        DtFim = null
                    };
                    await _contexto.BarbeiroServico.AddAsync(novoServicoBarbeiro);
                }

                await _contexto.SaveChangesAsync();
                await transaction.CommitAsync();
                return new GenericResponse { Sucesso = true};
            }
            catch
            {
                await transaction.RollbackAsync();
                return new GenericResponse { Sucesso = false, ErrorMessage = "Ocorreu um erro inesperado ao atualizar os serviços." };
            }
        }

        public async Task<ResultadoPaginado<ServicosDetalhesResponse>> ListarServicosBarbeiro(int idBarbeiro, PaginacaoFiltro request)
        {
            var query = _contexto.BarbeiroServico
                .Where(s =>  s.IdBarbeiro == idBarbeiro && s.DtFim == null)
                .Include(x => x.Servico)
                .ThenInclude(x => x!.CategoriaServico)
                .Select(s => new ServicosDetalhesResponse
                {
                    Id = s.Id,
                    Descricao = s.Servico!.Descricao,
                    Valor = s.Servico.Valor,
                    Categoria = s.Servico.CategoriaServico!.Descricao!
                })
                .AsQueryable();

            return await Paginacao.CriarPaginacao(query, request.Pagina, request.ItensPorPagina);
        }

        public async Task<ResultadoPaginado<ServicosDetalhesResponse>> ListarServicos(PaginacaoFiltro request)
        {
            var query = _contexto.Servico
                .Where(s => s.DtFim == null)
                .Include(x => x.CategoriaServico)
                .Select(s => new ServicosDetalhesResponse
                {
                    Id = s.Id,
                    Descricao = s.Descricao,
                    Valor = s.Valor,
                    Categoria = s.CategoriaServico!.Descricao!
                })
                .AsQueryable();

            return await Paginacao.CriarPaginacao(query, request.Pagina, request.ItensPorPagina);
        }
    }
}
