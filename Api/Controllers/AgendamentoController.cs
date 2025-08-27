using Api.Aplicacao.Contratos;
using Api.Modelos.Entidades;
using Api.Modelos.Request;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
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

        [HttpGet("ListarAgendamentos")]
        public IActionResult ListarAgendamentos([FromHeader] int idBarbeiro, [FromHeader]int idServico, [FromQuery] string nomeCliente,  [FromQuery] DateTime? dtInicio, [FromQuery] DateTime? dtFim)
        {
            idBarbeiro = idBarbeiro == 0 ? GetUserId() : idBarbeiro;
            var data = _app.ListarAgendamentos(idBarbeiro,idServico,nomeCliente, dtInicio, dtFim);
            return Ok(data);
        }

        [HttpPatch("CompletarAgendamento")]
        public IActionResult CompletarAgendamento(CompletarAgendamentoRequest request)
        {
            var data = _app.CompletarAgendamento(request);
            return Ok(data);
        }

    }
}
