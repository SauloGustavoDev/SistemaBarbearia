namespace Api.Modelos.Request
{
    public class ServicoBarbeiroEditarRequest
    {
        public int IdBarbeiro { get; set; }
        public List<int> IdsServico { get; set; } = new List<int>();
    }
}
