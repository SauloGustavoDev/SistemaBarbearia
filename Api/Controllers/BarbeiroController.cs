using Api.Aplicacao.Contratos;
using Api.Modelos.Dtos;
using Api.Models.Entity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarbeiroController : ControllerBase
    {
        public readonly IBarbeiroApp _app;

        public BarbeiroController(IBarbeiroApp app)
        {
            _app = app;
        }

        [HttpPost("Barbeiro")]
        public IActionResult CriarBarbeiro([FromBody] BarbeiroDto barbeiro)
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
