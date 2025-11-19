using Api.Aplicacao.Contratos;
using Api.Modelos.Request;
using Api.Modelos.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult<List<BarbeiroHorarioResponse>> HorariosBarbeiro([FromBody] BarbeiroHorarioRequest request)
        {
            var data =  _app.HorariosBarbeiro(request);
            return Ok(data);
        }
        [HttpPost("Agendamento")]
        public ActionResult GerarAgendamento([FromBody] AgendamentoCriarRequest request)
        {
            _app.CriarAgendamento(request);
            return Ok();
        }
        [HttpPost("Token")]
        public ActionResult GerarToken([FromBody] string numero)
        {
            _app.GerarToken(numero);
            return Ok();
        }
        [Authorize]
        [HttpGet("AgendamentoAtual")]
        public ActionResult<AgendamentoAtualResponse> AgendamentoAtual()
        {
            var data = _app.AgendamentoAtual(GetUserId());
            return data;
        }
        [Authorize]
        [HttpPut("Agendamento")]
        public ActionResult AtualizarAgendamento([FromBody] AgendamentoAtualizarRequest agendamento)
        {
            _app.AtualizarAgendamento(agendamento);
            return Ok();
        }
        [Authorize]
        [HttpGet("Agendamentos")]
        public ActionResult<ResultadoPaginado<AgendamentosDetalheResponse>> ListarAgendamentos([FromQuery] AgendamentoListarRequest request)
        {
            request.IdBarbeiro = request.IdBarbeiro == null ? GetUserId() : request.IdBarbeiro;
            var data =  _app.ListarAgendamentos(request);
            return data;
        }
        [Authorize]
        [HttpPatch("CompletarAgendamento")]
        public ActionResult CompletarAgendamento([FromBody]AgendamentoCompletarRequest request)
        {
            _app.CompletarAgendamento(request);
            return Ok();
        }
        [Authorize]
        [HttpDelete("Agendamento")]
        public ActionResult CancelarAgendamento([FromHeader]int id)
        {
            _app.CancelarAgendamento(id);
            return Ok();
        }

    }
}
