using Api.Aplicacao.Contratos;
using Api.Modelos.Paginacao;
using Api.Modelos.Request;
using Api.Modelos.Response;
using Microsoft.AspNetCore.Authorization;
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
        public ActionResult<ResultadoPaginado<ServicosDetalhesResponse>> ServicosBarbeiro([FromQuery] PaginacaoFiltro request)
        {
            var data = _app.ListarServicos(request);
            return data;
        }

        [HttpGet("ServicosBarbeiro")]
        public ActionResult<ResultadoPaginado<ServicosDetalhesResponse>> ServicosBarbeiro([FromHeader] int idBarbeiro, [FromQuery] PaginacaoFiltro request)
        {
            idBarbeiro = idBarbeiro == 0 ? GetUserId() : idBarbeiro;
            var data = _app.ListarServicosBarbeiro(idBarbeiro, request);
            return data;
        }
        [Authorize]
        [HttpPatch("ServicosBarbeiro")]
        public ActionResult ServicosBarbeiro([FromBody] ServicoBarbeiroEditarRequest request)
        {
            request.IdBarbeiro = request.IdBarbeiro == 0 ? GetUserId() : request.IdBarbeiro;
            _app.EditarServicosBarbeiro(request);
            return Ok();
        }
        [Authorize]
        [HttpPost("Servico")]
        public ActionResult CriarServico([FromBody] ServicoCriarRequest request)
        {
            _app.CriarServico(request);
            return Ok();
        }
        [Authorize]
        [HttpDelete("Servico")]
        public ActionResult DeletarServico([FromHeader] int id)
        {
            _app.DeletarServico(id);
            return Ok();
        }
        [Authorize]
        [HttpGet("Categorias")]
        public ActionResult<List<CategoriasResponse>> ListarCategorias()
        {
            return _app.ListarCategorias();
        }
        [Authorize]
        [HttpPut("Servico")]
        public ActionResult AtualizarServico([FromBody] ServicoAtualizarRequest request)
        {
            _app.AtualizarServico(request);
            return Ok();
        }
        [Authorize]
        [HttpPost("CategoriaServico")]
        public ActionResult CriarCategoriaServico([FromBody] string request)
        {
            _app.CriarCategoriaServico(request);
            return Ok();
        }

    }
}
