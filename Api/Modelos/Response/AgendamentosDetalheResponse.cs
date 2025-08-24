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
    public List<TimeOnly> Horarios { get; set; }
    public List<ServicosDetalhesResponse> Servicos { get; set; }

    public AgendamentosDetalheResponse(Agendamento request)
    {
        Id = request.Id;
        NomeCliente = request.NomeCliente;
        NumeroCliente = request.NumeroCliente;
        Status = request.Status;
        Horarios = request.AgendamentoHorarios.Select(x => x.BarbeiroHorario.Hora).ToList();
        Servicos = request.AgendamentoServicos.Select(x => new ServicosDetalhesResponse {Descricao = x.Servico.Descricao, Valor = x.Servico.Valor }).ToList();
    }
}
}
