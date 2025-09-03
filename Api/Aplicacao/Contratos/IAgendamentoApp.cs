using Api.Modelos.Entidades;
using Api.Modelos.Request;
using Api.Modelos.Response;

namespace Api.Aplicacao.Contratos
{
    public interface IAgendamentoApp
    {
        List<BarbeiroHorarioResponse> HorariosBarbeiro(BarbeiroHorarioRequest request);
        GenericResponse CriarAgendamento(AgendamentoCriarRequest request);
        List<AgendamentoResponse> ListarAgendamentos(int idBarbeiro,int idServico, string nomeCliente, DateTime? dtInicio, DateTime? dtFim, int status);
        GenericResponse CompletarAgendamento(AgendamentoCompletarRequest request);
        GenericResponse CancelarAgendamento(int id);
        GenericResponse AtualizarAgendamento(AgendamentoAtualizarRequest agendamento);
        AgendamentoAtualResponse AgendamentoAtual(int idBarbeiro);
    }
}
