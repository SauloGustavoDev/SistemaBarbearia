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
        public async Task<ActionResult<GenericResponse>> CriarBarbeiro([FromBody] BarbeiroCriarRequest barbeiro)
        {
            var data = await _app.Cadastrar(barbeiro);
            return data;
        }
        [HttpPut("Barbeiro")]
        public async Task<ActionResult<GenericResponse>> AtualizarBarbeiro(BarbeiroEditarRequest barbeiro)
        {
            var data = await _app.Editar(barbeiro);
            return data;
        }

        [HttpDelete("Barbeiro")]
        public async Task<ActionResult<GenericResponse>> ExcluirBarbeiro(int id)
        {
            var data = await _app.Excluir(id);
            return data;
        }

        [HttpGet("Barbeiro")]
        public async Task<ActionResult<BarbeiroDetalhesResponse>> GetBarbeiro()
        {
            var data = await _app.BarbeiroDetalhes(GetUserId());
            return data;
        }
        [HttpGet("Barbeiros")]
        public async Task<ActionResult<ResultadoPaginado<BarbeiroDetalhesResponse>>> GetBarbeiros([FromQuery] PaginacaoFiltro request) 
        {
            var data = await _app.ListaBarbeiros(request);
            return data;
        }

    }
}
