using Api.Aplicacao.Contratos;
using Api.Modelos.Response;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RelatorioController(IRelatorioApp app) : GsSystemControllerBase
    {
        private readonly IRelatorioApp _app = app;
        public ActionResult<RelatorioFinanceiro> RelatorioFinanceiro()
        {
            return Ok();
        }
    }
}
