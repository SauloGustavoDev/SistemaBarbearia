using Api.Aplicacao.Contratos;
using Api.Infraestrutura.Contexto;

namespace Api.Aplicacao.Servicos
{
    public class ExcecaoApp(Contexto contexto) : IExcecaoApp
    {
        private readonly Contexto _contexto = contexto;

        public void CadastrarExcecao()
        {
            
        }

        public void ListarExcecoes()
        {
            throw new NotImplementedException();
        }

        public void RemoverExcecao()
        {
            throw new NotImplementedException();
        }
    }
}
