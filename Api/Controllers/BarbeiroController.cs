using Api.Aplicacao.Contratos;
using Api.Modelos.Dtos;
using Api.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarbeiroController : GsSystemControllerBase
    {
        public readonly IBarbeiroApp _app;

        public BarbeiroController(IBarbeiroApp app)
        {
            _app = app;
        }

        [HttpPost("Barbeiro")]
        public IActionResult CriarBarbeiro([FromBody] BarbeiroCriarRequest barbeiro)
        {
            _app.Cadastrar(barbeiro);
            return Ok();
        }
        [HttpPut("Barbeiro")]
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

        [HttpGet("Barbeiro")]
        public IActionResult GetBarbeiro()
        {
            var result = _app.BarbeiroDetalhes(GetUserId());
            return Ok(result);
        }
        [HttpGet("Barbeiros")]
        public IActionResult GetBarbeiros() 
        {
            var result = _app.ListaBarbeiros();
            return Ok(result);
        }

    }
}
