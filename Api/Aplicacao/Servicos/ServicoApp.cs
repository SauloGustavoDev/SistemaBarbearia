using Api.Aplicacao.Contratos;
using Api.Infraestrutura.Contexto;
using Api.Modelos.Response;

namespace Api.Aplicacao.Servicos
{
    public class ServicoApp : IServicoApp
    {
        public readonly Contexto _contexto;
        public ServicoApp(Contexto contexto)
        {
            _contexto = contexto;
        }

        public List<ServicosDetalhesResponse> ListarServicos(int idBarbeiro)
        {
            var servicos = _contexto.Servico
                .Where(s =>  s.IdBarbeiro == idBarbeiro)
                .Select(s => new ServicosDetalhesResponse
                {
                    Id = s.Id,
                    Descricao = s.Descricao,
                    Valor = s.Valor
                })
                .ToList();

            return servicos;
        }
    }
}
