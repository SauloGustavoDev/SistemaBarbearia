using Api.Modelos.Entidades;
using Api.Modelos.Enums;

namespace Api.Modelos.Response
{
 public class AgendamentosDetalheResponse
{
    public int Id { get; set; }
    public string NomeCliente { get; set; }
    public string NumeroCliente { get; set; }
    public Status Status { get; set; }
    public TimeOnly Horario { get; set; }
    public DateTime Data { get; set; }
    public List<ServicosDetalhesResponse> Servicos { get; set; }

        public AgendamentosDetalheResponse(Agendamento request)
    {
        Id = request.Id;
        NomeCliente = request.NomeCliente;
        NumeroCliente = request.NumeroCliente;
        Status = request.Status;
        Horario = request.AgendamentoHorarios.OrderBy(x => x.BarbeiroHorario!.Hora).FirstOrDefault()!.BarbeiroHorario!.Hora;
        Data = request.DtAgendamento.Date;
        Servicos = request.AgendamentoServicos.Select(x => new ServicosDetalhesResponse {Descricao = x.Servico!.Descricao, Valor = x.Servico.Valor}).ToList();
    }
}
}
