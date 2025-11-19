using Api.Aplicacao.Contratos;
using Api.Modelos.Request;
using Api.Modelos.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RelatorioController(IRelatorioApp app) : GsSystemControllerBase
    {
        private readonly IRelatorioApp _app = app;
        [Authorize]
        [HttpGet("RelatorioFinanceiro")]
        public ActionResult<RelatorioFinanceiro> RelatorioFinanceiro([FromQuery] RelatorioFinanceiroRequest request)
        {
            request.IdBarbeiro = request.IdBarbeiro != 0 ? request.IdBarbeiro : GetUserId();
            return _app.GerarRelatorioFinanceiro(request);
        }
    }
}
