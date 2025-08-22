using Api.Aplicacao.Contratos;
using Api.Modelos.Dtos;
using Api.Modelos.Request;
using Api.Models.Entity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoController : GsSystemControllerBase
    {
        public readonly IAutenticacaoApp _app;

        public AutenticacaoController(IAutenticacaoApp app)
        {
            _app = app;
        }
        [HttpPost("Login")]
        public IActionResult Login([FromBody] BarbeiroLoginRequest login)
        {
            var result = _app.Login(login);

            if (!result.Sucesso)
                return Unauthorized(new { message = result.ErrorMessage });

            return Ok(new { result.Token });

        }

        [HttpPost("EsqueceuSenha")]
        public IActionResult EsqueceuSenha([FromBody] BarbeiroEsqueceSenhaRequest request)
        {
            var result = _app.EsqueceuSenha(request);

            if (!result.Sucesso)
                return Unauthorized(new { message = result.ErrorMessage });

            return Ok();

        }

        [HttpPatch("NovaSenha")]
        public IActionResult EditarSenhaBarbeiro(string novaSenha)
        {
            _app.AtualizarSenha(GetUserId(),novaSenha);
            return Ok();
        }

    }
}
