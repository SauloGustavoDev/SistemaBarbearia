namespace Api.Modelos.Request
{
    public class BarbeiroHorarioRequest
    {
        public int Id { get; set; }
        public List<int> IdsServico { get; set; } = new List<int>();
    }
}
