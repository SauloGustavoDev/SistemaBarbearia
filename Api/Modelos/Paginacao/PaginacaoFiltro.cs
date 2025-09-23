namespace Api.Modelos.Paginacao
{
    public class PaginacaoFiltro
    {
        public int Pagina { get; set; } = 1;
        public int ItensPorPagina { get; set; } = 10;
    }
}
