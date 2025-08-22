using Api.Modelos.Request;
using Api.Modelos.Response;

namespace Api.Aplicacao.Contratos
{
    public interface IAgendamentoApp
    {
        List<BarbeiroHorarioResponse> HorariosBarbeiro(BarbeiroHorarioRequest request);
        GenericResponse CriarAgendamento(AgendamentoCriarRequest request);
        AgendamentoResponse ListarAgendamentos(int idBarbeiro, DateTime? data);
    }
}
