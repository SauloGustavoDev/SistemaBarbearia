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
            return _app.Login(login);
        }

        [HttpPost("EsqueceuSenha")]
        public ActionResult EsqueceuSenha([FromBody] BarbeiroEsqueceSenhaRequest request)
        {
            _app.EsqueceuSenha(request);
            return Ok();
        }

        [HttpPatch("NovaSenha")]
        public ActionResult EditarSenhaBarbeiro(string novaSenha)
        {
            _app.AtualizarSenha(GetUserId(),novaSenha);
            return Ok();
        }
    }
}
