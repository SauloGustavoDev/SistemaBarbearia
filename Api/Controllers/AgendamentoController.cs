using Api.Aplicacao.Contratos;
using Api.Modelos.Entidades;
using Api.Modelos.Request;
using Api.Modelos.Response;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AgendamentoController : GsSystemControllerBase
    {
        private readonly IAgendamentoApp _app;
        public AgendamentoController(IAgendamentoApp app)
        {
            _app = app;
        }
        [HttpPost("HorariosBarbeiros")]
        public async Task<ActionResult<List<BarbeiroHorarioResponse>>> HorariosBarbeiro([FromBody] BarbeiroHorarioRequest request)
        {
            var data = await _app.HorariosBarbeiro(request);
            return Ok(data);
        }
        [HttpPost("Agendamento")]
        public async Task<ActionResult<GenericResponse>> GerarAgendamento([FromBody] AgendamentoCriarRequest request)
        {
            var data = await _app.CriarAgendamento(request);
            return data;
        }
        [HttpPost("Token")]
        public async Task<ActionResult> GerarToken([FromBody] string numero)
        {
            var data = await _app.GerarToken(numero);
            return Ok(data);
        }

        [HttpGet("AgendamentoAtual")]
        public async Task<ActionResult<AgendamentoAtualResponse>> AgendamentoAtual()
        {
            var data =await _app.AgendamentoAtual(GetUserId());
            return data;
        }

        [HttpPut("Agendamento")]
        public async Task<ActionResult<GenericResponse>> AtualizarAgendamento([FromBody] AgendamentoAtualizarRequest agendamento)
        {
            var data =await _app.AtualizarAgendamento(agendamento);
            return data;
        }

        [HttpGet("Agendamentos")]
        public async Task<ActionResult<ResultadoPaginado<AgendamentosDetalheResponse>>> ListarAgendamentos([FromQuery] AgendamentoListarRequest request)
        {
            request.IdBarbeiro = request.IdBarbeiro == 0 ? GetUserId() : request.IdBarbeiro;
            var data = await _app.ListarAgendamentos(request);
            return data;
        }

        [HttpPatch("CompletarAgendamento")]
        public async Task<ActionResult<GenericResponse>> CompletarAgendamento([FromBody]AgendamentoCompletarRequest request)
        {
            var data = await _app.CompletarAgendamento(request);
            return data;
        }
        [HttpDelete("Agendamento")]
        public async Task<ActionResult<GenericResponse>> CancelarAgendamento([FromHeader]int id)
        {
            var data = await _app.CancelarAgendamento(id);
            return data;
        }

    }
}
