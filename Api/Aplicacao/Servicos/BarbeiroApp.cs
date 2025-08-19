using Api.Aplicacao.Contratos;
using Api.Infraestrutura.Contexto;
using Api.Modelos.Dtos;
using Api.Models.Entity;

namespace Api.Aplicacao.Servicos
{
    public class BarbeiroApp : IBarbeiroApp
    {
        public readonly Contexto _contexto;
        public BarbeiroApp(Contexto contexto)
        {
            _contexto = contexto;
        }
        public void Cadastrar(BarbeiroCriarRequest barbeiro)
        {
            _contexto.Add(new Barbeiro(barbeiro));
            _contexto.SaveChanges();
        }

        public void Editar(Barbeiro barbeiro)
        {
            _contexto.Update(barbeiro);
            _contexto.SaveChanges();
        }

        public void Excluir(int id)
        {
            _contexto.Remove(new Barbeiro {Id = id });
            _contexto.SaveChanges();
        }
    }
}
