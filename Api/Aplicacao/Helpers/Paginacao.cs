using Api.Modelos.Response;
using Microsoft.EntityFrameworkCore;

namespace Api.Aplicacao.Helpers
{
    public static class Paginacao
    {
        public static async Task<ResultadoPaginado<T>> CriarPaginacao<T>(IQueryable<T> query, int pagina, int itensPorPagina)
        {
            var total = await query
                .CountAsync();
            var itens = await query
                .Skip((pagina - 1) * itensPorPagina)
                .Take(itensPorPagina)
                .ToListAsync();

            return new ResultadoPaginado<T>
            {
                Items = itens,
                TotalRegistros = total,
                PaginaAtual = pagina,
                ItensPorPagina = itensPorPagina
            };
        }
    }
}
