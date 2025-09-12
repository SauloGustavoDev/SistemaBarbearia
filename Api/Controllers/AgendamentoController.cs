using Api.Aplicacao.Contratos;
using Api.Modelos.Entidades;
using Api.Modelos.Request;
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
        public IActionResult HorariosBarbeiro([FromBody] BarbeiroHorarioRequest request)
        {
            var data = _app.HorariosBarbeiro(request);
            return Ok(data);
        }
        [HttpPost("Agendamento")]
        public IActionResult GerarAgendamento([FromBody] AgendamentoCriarRequest request)
        {
            var data = _app.CriarAgendamento(request);
            return Ok(data);
        }
        [HttpGet("AgendamentoAtual")]
        public IActionResult AgendamentoAtual()
        {
            var data = _app.AgendamentoAtual(GetUserId());
            return Ok(data);
        }

        [HttpPut("Agendamento")]
        public IActionResult AtualizarAgendamento([FromBody] AgendamentoAtualizarRequest agendamento)
        {
            var data = _app.AtualizarAgendamento(agendamento);
            return Ok(data);
        }

        [HttpGet("Agendamentos")]
        public IActionResult ListarAgendamentos([FromHeader] int idBarbeiro, [FromHeader]int idServico, [FromQuery] string nomeCliente,  [FromQuery] DateTime? dtInicio, [FromQuery] DateTime? dtFim, int status)
        {
            idBarbeiro = idBarbeiro == 0 ? GetUserId() : idBarbeiro;
            var data = _app.ListarAgendamentos(idBarbeiro,idServico,nomeCliente, dtInicio, dtFim, status);
            return Ok(data);
        }

        [HttpPatch("CompletarAgendamento")]
        public IActionResult CompletarAgendamento([FromBody]AgendamentoCompletarRequest request)
        {
            var data = _app.CompletarAgendamento(request);
            return Ok(data);
        }
        [HttpDelete("Agendamento")]
        public IActionResult CancelarAgendamento([FromHeader]int id)
        {
            var data = _app.CancelarAgendamento(id);
            return Ok(data);
        }

    }
}
