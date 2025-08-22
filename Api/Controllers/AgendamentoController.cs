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
        [HttpPost("HorariosBarbeiro")]
        public IActionResult HorariosBarbeiro([FromBody] BarbeiroHorarioRequest request)
        {
            if (request.IdsServico.Count == 0)
                return BadRequest("Selecione algum serviço");

            var data = _app.HorariosBarbeiro(request);
            return Ok(data);
        }
        [HttpPost("Agendamento")]
        public IActionResult GerarAgendamento(AgendamentoCriarRequest request)
        {
            _app.CriarAgendamento(request);
            return Ok();
        }
    }
}
