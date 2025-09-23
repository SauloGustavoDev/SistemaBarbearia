using Api.Aplicacao.Contratos;
using Api.Modelos.Request;
using Api.Modelos.Response;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoController(IAutenticacaoApp app) : GsSystemControllerBase
    {
        public readonly IAutenticacaoApp _app = app;

        [HttpPost("Login")]
        public ActionResult<string> Login([FromBody] BarbeiroLoginRequest login)
        {
            var result = _app.Login(login);

            if (!result.Sucesso)
                return Unauthorized(new { message = result.ErrorMessage });

            return result.Token!;

        }

        [HttpPost("EsqueceuSenha")]
        public async Task<ActionResult<GenericResponse>> EsqueceuSenha([FromBody] BarbeiroEsqueceSenhaRequest request)
        {
            var result =await _app.EsqueceuSenha(request);

            if (!result.Sucesso)
                return Unauthorized(new { message = result.ErrorMessage });

            return result;

        }

        [HttpPatch("NovaSenha")]
        public async Task<ActionResult<GenericResponse>> EditarSenhaBarbeiro(string novaSenha)
        {
            var result = await _app.AtualizarSenha(GetUserId(),novaSenha);
            return result;
        }

    }
}
