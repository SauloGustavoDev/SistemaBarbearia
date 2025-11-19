using Api.Aplicacao.Contratos;
using Api.Modelos.Request;
using Api.Modelos.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MensalistaController(IMensalistaApp app) : GsSystemControllerBase
    {
        private readonly IMensalistaApp _app = app;
        [Authorize]
        [HttpPost]
        public IActionResult CadastrarMensalista([FromBody] MensalistaCriarRequest request)
        {
            _app.CadastrarMensalista(request, GetUserId());
            return Ok();
        }
        [Authorize]
        [HttpDelete]
        public IActionResult CancelarMensalista([FromBody] int idMensalista)
        {
            _app.CancelarMensalista(idMensalista);
            return Ok();
        }
        [Authorize]
        [HttpGet]
        public ActionResult<List<MensalistaDetalhesResponse>> ListarMensalistas()
        {
            var id = GetUserId();
            return _app.ListarMensalistas(id);
        }
    }
}
