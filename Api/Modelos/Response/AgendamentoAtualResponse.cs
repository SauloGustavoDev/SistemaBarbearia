using Api.Modelos.Entidades;

namespace Api.Modelos.Response
{
    public class AgendamentoAtualResponse
    {
        public int Id { get; set; }
        public string Barbeiro { get; set; }
        public string Cliente { get; set; }
        public DateTime Dt_Agendamento { get; set; }
        public List<ServicosDetalhesResponse> Servicos { get; set; }
        public decimal ValorTotal { get; set; }

        public AgendamentoAtualResponse(Agendamento agendamento)
        {
            Id = agendamento.Id;
            Barbeiro = agendamento.Barbeiro!.Nome;
            Cliente = agendamento.NomeCliente;
            var data = DateOnly.FromDateTime(agendamento.DtAgendamento);
            var hora = agendamento.AgendamentoHorarios
                        .Select(x => x.BarbeiroHorario!.Hora)
                        .OrderBy(h => h)
                        .FirstOrDefault();
            Dt_Agendamento = new DateTime(data, hora);
            Servicos = agendamento.AgendamentoServicos.Select(s => new ServicosDetalhesResponse
            {
                Id = s.Servico!.Id,
                Descricao = s.Servico.Descricao,
                Valor = s.Servico.Valor
            }).ToList();
            ValorTotal = Servicos.Sum(s => s.Valor);
        }
    }
}
