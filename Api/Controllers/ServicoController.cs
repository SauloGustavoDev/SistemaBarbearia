using Api.Aplicacao.Contratos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicoController : GsSystemControllerBase
    {
        private readonly IServicoApp _app;
        public ServicoController(IServicoApp app)
        {
            _app = app;
        }

        [HttpGet("Servicos")]
        public IActionResult ListarServicos([FromHeader] int idBarbeiro)
        {
            idBarbeiro = idBarbeiro == 0 ? GetUserId() : idBarbeiro;
            var data = _app.ListarServicos(idBarbeiro);
            return Ok(data);
        }
    }
}
