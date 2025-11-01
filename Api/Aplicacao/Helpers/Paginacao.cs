using Api.Modelos.Response;
using Microsoft.EntityFrameworkCore;

namespace Api.Aplicacao.Helpers
{
    public static class Paginacao
    {
        public static ResultadoPaginado<T> CriarPaginacao<T>(IQueryable<T> query, int pagina, int itensPorPagina)
        {
            var total = query
                .Count();
            var itens = query
                .Skip((pagina - 1) * itensPorPagina)
                .Take(itensPorPagina)
                .ToList();

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
