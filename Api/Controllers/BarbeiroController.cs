using Api.Aplicacao.Contratos;
using Api.Models.Entity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarbeiroController : ControllerBase
    {
        private readonly IBarbeiroApp _app;

        public BarbeiroController(IBarbeiroApp app)
        {
            _app = app;
        }

        [HttpPost("Barbeiro")]
        public IActionResult CriarBarbeiro([FromBody]Barbeiro barbeiro)
        {
            _app.Cadastrar(barbeiro);
            return Ok();
        }
        [HttpPatch("Barbeiro")]
        public IActionResult AtualizarBarbeiro(Barbeiro barbeiro)
        {
            _app.Editar(barbeiro);
            return Ok();
        }
        [HttpDelete("Barbeiro")]
        public IActionResult ExcluirBarbeiro(int id)
        {
            _app.Excluir(id);
            return Ok();
        }
    }
}
