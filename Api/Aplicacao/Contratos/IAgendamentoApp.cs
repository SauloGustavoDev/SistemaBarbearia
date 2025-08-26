using Api.Modelos.Request;
using Api.Modelos.Response;

namespace Api.Aplicacao.Contratos
{
    public interface IAgendamentoApp
    {
        List<BarbeiroHorarioResponse> HorariosBarbeiro(BarbeiroHorarioRequest request);
        GenericResponse CriarAgendamento(AgendamentoCriarRequest request);
        List<AgendamentoResponse> ListarAgendamentos(int idBarbeiro, DateTime? dtInicio, DateTime? dtFim);
        GenericResponse CompletarAgendamento(CompletarAgendamentoRequest request);
    }
}
