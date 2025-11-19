using Api.Aplicacao.Contratos;
using Api.Modelos.Dtos;
using Api.Modelos.Paginacao;
using Api.Modelos.Request;
using Api.Modelos.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarbeiroController(IBarbeiroApp app) : GsSystemControllerBase
    {
        public readonly IBarbeiroApp _app = app;
        [Authorize]
        [HttpPost("Barbeiro")]
        public ActionResult CriarBarbeiro([FromBody] BarbeiroCriarRequest barbeiro)
        {
            _app.Cadastrar(barbeiro);
            return Ok();
        }
        [Authorize]
        [HttpPut("Barbeiro")]
        public ActionResult AtualizarBarbeiro(BarbeiroEditarRequest barbeiro)
        {
            _app.Editar(barbeiro);
            return Ok();
        }
        [Authorize]
        [HttpDelete("Barbeiro")]
        public ActionResult ExcluirBarbeiro(int id)
        {
            _app.Excluir(id);
            return Ok();
        }
        [Authorize]
        [HttpGet("Barbeiro")]
        public ActionResult<BarbeiroDetalhesResponse> GetBarbeiro()
        {
            var data = _app.BarbeiroDetalhes(GetUserId());
            return data;
        }
        [HttpGet("Barbeiros")]
        public ActionResult<ResultadoPaginado<BarbeiroDetalhesResponse>> GetBarbeiros([FromQuery] PaginacaoFiltro request) 
        {
            var data = _app.ListaBarbeiros(request);
            return data;
        }

    }
}
