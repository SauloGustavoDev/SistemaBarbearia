using Api.Aplicacao.Contratos;
using Api.Modelos.Dtos;
using Api.Modelos.Request;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoController : ControllerBase
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

            if (!result.Successo)
                return Unauthorized(new { message = result.ErrorMessage });

            return Ok(new { result.Token });

        }

        [HttpPost("NovaSenha")]
        public IActionResult NovaSenha([FromBody] BarbeiroLoginRequest login)
        {
            var result = _app.Login(login);

            if (!result.Successo)
                return Unauthorized(new { message = result.ErrorMessage });

            return Ok(new { result.Token });

        }
        /*[HttpPost("EsqueceuSenha")]
        public IActionResult EsqueceuSenha([FromBody] BarbeiroEsqueceSenhaRequest request)
        {
            var result = _app.Login(login);

            if (!result.Successo)
                return Unauthorized(new { message = result.ErrorMessage });

            return Ok(new { result.Token });

        }
        [HttpPost("Sair")]
        public IActionResult Sair()
        {
            var result = _app.Login(login);

            if (!result.Successo)
                return Unauthorized(new { message = result.ErrorMessage });

            return Ok(new { result.Token });

        }*/

    }
}
