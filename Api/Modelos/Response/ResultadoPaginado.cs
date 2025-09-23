namespace Api.Modelos.Response
{
    public class ResultadoPaginado<T>
    {
        public List<T>? Items { get; set; }
        public int TotalRegistros { get; set; }
        public int PaginaAtual { get; set; }
        public int ItensPorPagina { get; set; }
    }
}
