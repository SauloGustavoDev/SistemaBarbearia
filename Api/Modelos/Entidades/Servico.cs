using Api.Modelos.Enums;

namespace Api.Modelos.Entidades
{
    public class Servico
    {
        private int Id { get; set; }
        private Servicos Descricao { get; set; }
        private decimal Valor { get; set; }
    }
}
