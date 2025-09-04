using Api.Aplicacao.Contratos;
using Api.Modelos.Request;
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
        [HttpPatch("ServicosBarbeiro")]
        public IActionResult ServicosBarbeiro([FromBody] ServicoBarbeiroEditarRequest request)
        {
            request.IdBarbeiro = request.IdBarbeiro == 0 ? GetUserId() : request.IdBarbeiro;
            var data = _app.EditarServicosBarbeiro(request);
            return Ok(data);
        }
        [HttpPost("Servico")]
        public IActionResult CriarServico([FromBody] ServicoCriarRequest request)
        {
            var data = _app.CriarServico(request);
            return Ok(data);
        }
        [HttpDelete("Servico")]
        public IActionResult DeletarServico([FromHeader] int id)
        {
            var data = _app.DeletarServico(id);
            return Ok(data);
        }
        [HttpPut("Servico")]
        public IActionResult AtualizarServico([FromBody] ServicoAtualizarRequest request)
        {
            var data = _app.AtualizarServico(request);
            return Ok(data);
        }
    }
}
