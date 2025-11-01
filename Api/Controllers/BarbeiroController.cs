using Api.Aplicacao.Contratos;
using Api.Modelos.Dtos;
using Api.Modelos.Paginacao;
using Api.Modelos.Request;
using Api.Modelos.Response;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarbeiroController(IBarbeiroApp app) : GsSystemControllerBase
    {
        public readonly IBarbeiroApp _app = app;

        [HttpPost("Barbeiro")]
        public ActionResult CriarBarbeiro([FromBody] BarbeiroCriarRequest barbeiro)
        {
            _app.Cadastrar(barbeiro);
            return Ok();
        }
        [HttpPut("Barbeiro")]
        public ActionResult AtualizarBarbeiro(BarbeiroEditarRequest barbeiro)
        {
            _app.Editar(barbeiro);
            return Ok();
        }

        [HttpDelete("Barbeiro")]
        public ActionResult ExcluirBarbeiro(int id)
        {
            _app.Excluir(id);
            return Ok();
        }

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
