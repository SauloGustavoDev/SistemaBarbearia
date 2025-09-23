using Api.Modelos.Entidades;
using Api.Modelos.Request;
using Api.Modelos.Response;

namespace Api.Aplicacao.Contratos
{
    public interface IAgendamentoApp
    {
        Task<List<BarbeiroHorarioResponse>> HorariosBarbeiro(BarbeiroHorarioRequest request);
        Task<GenericResponse> CriarAgendamento(AgendamentoCriarRequest request);
        Task<ResultadoPaginado<AgendamentosDetalheResponse>> ListarAgendamentos(AgendamentoListarRequest request);
        Task<GenericResponse> CompletarAgendamento(AgendamentoCompletarRequest request);
        Task<GenericResponse> CancelarAgendamento(int id);
        Task<GenericResponse> AtualizarAgendamento(AgendamentoAtualizarRequest agendamento);
        Task<AgendamentoAtualResponse> AgendamentoAtual(int idBarbeiro);
    }
}
