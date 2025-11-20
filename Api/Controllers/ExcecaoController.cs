using Api.Aplicacao.Contratos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExcecaoController(IExcecaoApp app) : GsSystemControllerBase
    {
        private readonly IExcecaoApp _app = app;
        [HttpPost]
        public IActionResult CadastrarExcecao()
        {

            _app.CadastrarExcecao();
            return Ok();
        }
    }
}
