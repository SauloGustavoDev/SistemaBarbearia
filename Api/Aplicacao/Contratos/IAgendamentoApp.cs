using Api.Modelos.Entidades;
using Api.Modelos.Request;
using Api.Modelos.Response;

namespace Api.Aplicacao.Contratos
{
    public interface IAgendamentoApp
    {
        List<BarbeiroHorarioResponse> HorariosBarbeiro(BarbeiroHorarioRequest request);
        void CriarAgendamento(AgendamentoCriarRequest request);
        ResultadoPaginado<AgendamentosDetalheResponse> ListarAgendamentos(AgendamentoListarRequest request);
        void CompletarAgendamento(AgendamentoCompletarRequest request);
        void CancelarAgendamento(int id);
        void AtualizarAgendamento(AgendamentoAtualizarRequest agendamento);
        AgendamentoAtualResponse AgendamentoAtual(int idBarbeiro);
        void GerarToken(string numero);
    }
}
