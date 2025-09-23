using Api.Aplicacao.Contratos;
using Api.Modelos.Paginacao;
using Api.Modelos.Request;
using Api.Modelos.Response;
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
        public async Task<ActionResult<ResultadoPaginado<ServicosDetalhesResponse>>> ServicosBarbeiro([FromQuery] PaginacaoFiltro request)
        {
            var data =await _app.ListarServicos(request);
            return data;
        }

        [HttpGet("ServicosBarbeiro")]
        public async Task<ActionResult<ResultadoPaginado<ServicosDetalhesResponse>>> ServicosBarbeiro([FromHeader] int idBarbeiro, [FromQuery] PaginacaoFiltro request)
        {
            idBarbeiro = idBarbeiro == 0 ? GetUserId() : idBarbeiro;
            var data =await _app.ListarServicosBarbeiro(idBarbeiro, request);
            return data;
        }
        [HttpPatch("ServicosBarbeiro")]
        public async Task<ActionResult<GenericResponse>> ServicosBarbeiro([FromBody] ServicoBarbeiroEditarRequest request)
        {
            request.IdBarbeiro = request.IdBarbeiro == 0 ? GetUserId() : request.IdBarbeiro;
            var data =await _app.EditarServicosBarbeiro(request);
            return data;
        }
        [HttpPost("Servico")]
        public async Task<ActionResult<GenericResponse>> CriarServico([FromBody] ServicoCriarRequest request)
        {
            var data =await _app.CriarServico(request);
            return data;
        }
        [HttpDelete("Servico")]
        public async Task<ActionResult<GenericResponse>> DeletarServico([FromHeader] int id)
        {
            var data =await _app.DeletarServico(id);
            return data;
        }
        [HttpPut("Servico")]
        public async Task<ActionResult<GenericResponse>> AtualizarServico([FromBody] ServicoAtualizarRequest request)
        {
            var data =await _app.AtualizarServico(request);
            return data;
        }
        [HttpPost("CategoriaServico")]
        public async Task<ActionResult<GenericResponse>> CriarCategoriaServico([FromBody] string request)
        {
            var data =await _app.CriarCategoriaServico(request);
            return data;
        }

    }
}
