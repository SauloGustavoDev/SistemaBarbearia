using Api.Aplicacao.Contratos;
using Api.Modelos.Response;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestesController : GsSystemControllerBase
    {
        private readonly ITestesApp _app;

        public TestesController(ITestesApp app)
        {
            _app = app;
        }
        [HttpGet("GerarBancoSimulado")]
        public ActionResult GerarBancoSimulado()
        {
            _app.GerarBancoSimulado();
            return Ok();
        }

        [HttpGet("LimpaBancoSimulado")]
        public ActionResult LimpaBancoSimulado()
        {
            _app.LimparBancoDeDados();
            return Ok();
        }
    }
}
