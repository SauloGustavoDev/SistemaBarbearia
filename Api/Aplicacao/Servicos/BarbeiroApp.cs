using Api.Aplicacao.Contratos;
using Api.Infraestrutura.Contexto;
using Api.Modelos.Dtos;
using Api.Modelos.Entidades;
using Api.Modelos.Response;
using Api.Models.Entity;
using Microsoft.EntityFrameworkCore;

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

        public BarbeiroDetalhesResponse BarbeiroDetalhes(int id)
        {
            var barbeiro = _contexto.Set<Barbeiro>()
                           .AsNoTracking()
                           .Include(x => x.BarbeiroServicos)
                           .Include(x => x.Agendamentos)
                           .Include(x => x.BarbeiroHorarios)
                           .FirstOrDefault(x => x.Id == id) ?? throw new Exception("Barbeiro não encontrado");

            return new BarbeiroDetalhesResponse(barbeiro);
        }

        public List<BarbeiroDetalhesResponse> ListaBarbeiros()
        {
            var barbeiros = _contexto.Set<Barbeiro>()
                .AsNoTracking()
                .Where(x => x.DtDemissao == null)
                .ToList();

            if (barbeiros.Count == 0)
                throw new Exception("Nenhnum barbeiro encontrado");

            var data = new List<BarbeiroDetalhesResponse>();
            foreach (var barbeiro in barbeiros)
            {
                data.Add(new BarbeiroDetalhesResponse(barbeiro));
            }

            return data;
        }
    }
}
