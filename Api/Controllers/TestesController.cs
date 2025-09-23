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
        public async Task<ActionResult<GenericResponse>> GerarBancoSimulado()
        {
            var data = await _app.GerarBancoSimulado();
            return data;
        }

        [HttpGet("LimpaBancoSimulado")]
        public async Task<ActionResult<GenericResponse>> LimpaBancoSimulado()
        {
            var data = await _app.LimparBancoDeDados();
            return data;
        }
    }
}
