using Api.Aplicacao.Contratos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class TestesController : GsSystemControllerBase
    {
        private readonly ITestesApp _app;

        public TestesController(ITestesApp app)
        {
            _app = app;
        }
        [HttpGet("GerarBancoSimulado")]
        public IActionResult GerarBancoSimulado()
        {
            var data = _app.GerarBancoSimulado();
            return Ok(data);
        }

        [HttpGet("LimpaBancoSimulado")]
        public IActionResult LimpaBancoSimulado()
        {
            var data = _app.LimparBancoDeDados();
            return Ok(data);
        }
    }
}
