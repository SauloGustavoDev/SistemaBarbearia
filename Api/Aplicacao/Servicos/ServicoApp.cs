using Api.Aplicacao.Contratos;
using Api.Aplicacao.Helpers;
using Api.Infraestrutura.Contexto;
using Api.Modelos.Entidades;
using Api.Modelos.Request;
using Api.Modelos.Response;
using Microsoft.EntityFrameworkCore;

namespace Api.Aplicacao.Servicos
{
    public class ServicoApp : IServicoApp
    {
        public readonly Contexto _contexto;
        public ServicoApp(Contexto contexto)
        {
            _contexto = contexto;
        }


        public GenericResponse CriarServico(ServicoCriarRequest request)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Descricao))
                erros.Add("A descrição do serviço é obrigatória.");

            if (request.Valor <= 0)
                erros.Add("O valor do serviço deve ser maior que zero.");

            if (request.TempoEstimado == 0)
                erros.Add("O tempo estimado do serviço é obrigatório.");

            if (request.Categoria <= 0)
                erros.Add("A categoria do serviço é obrigatória.");

            if (erros.Any())
                return new GenericResponse { Sucesso = false, ErrorMessage = string.Join(" ", erros) };

            var categoriaExiste = _contexto.CategoriaServico.Any(c => c.Id == request.Categoria);
            if (!categoriaExiste)
                return new GenericResponse { Sucesso = false, ErrorMessage = $"A categoria com ID {request.Categoria} não foi encontrada." };

            var novoServico = new Servico
            {
                Descricao = request.Descricao,
                Valor = request.Valor,
                TempoEstimado = new TimeOnly(0, request.TempoEstimado, 0),
                DtInicio = DateTime.UtcNow,
                IdCategoriaServico = request.Categoria
            };

            try
            {
                _contexto.Servico.Add(novoServico);
                _contexto.SaveChanges();

                return new GenericResponse { Sucesso = true };
            }
            catch
            {
                return new GenericResponse { Sucesso = false, ErrorMessage = "Ocorreu um erro inesperado." };
            }
        }
        public GenericResponse AtualizarServico(ServicoAtualizarRequest request)
        {
            var servicoAtual = _contexto.Servico.FirstOrDefault(s => s.Id == request.Id && s.DtFim == null);

            if (servicoAtual == null)
                return new GenericResponse { Sucesso = false, ErrorMessage = $"Serviço ativo com ID {request.Id} não foi encontrado." };

            var categoriaExiste = _contexto.CategoriaServico.Any(c => c.Id == request.Categoria);
            if (!categoriaExiste)
                return new GenericResponse { Sucesso = false, ErrorMessage = $"A categoria com ID {request.Categoria} não foi encontrada." };

            servicoAtual.DtFim = DateTime.UtcNow;
            _contexto.Servico.Update(servicoAtual);

            var novoServico = new Servico
            {
                Descricao = request.Descricao,
                Valor = request.Valor,
                TempoEstimado = new TimeOnly(0,request.TempoEstimado,0),
                DtInicio = DateTime.UtcNow,
                IdCategoriaServico = request.Categoria
            };
            _contexto.Servico.Add(novoServico);
            try
            {
                _contexto.SaveChanges();
                return new GenericResponse { Sucesso = true};
            }
            catch
            {
                return new GenericResponse { Sucesso = false, ErrorMessage = "Ocorreu um erro inesperado ao atualizar o serviço." };
            }
        }

        public GenericResponse CriarCategoriaServico(string request)
        {
            var novaCategoria = new CategoriaServico
            {
                Descricao = request,
                DtInicio = DateTime.UtcNow
            };
            return MontarGenericResponse.TryExecute(() =>
            {
                _contexto.CategoriaServico.Add(novaCategoria);
                _contexto.SaveChanges();
            }, "Ocorreu um erro inesperado ao criar a categoria.");
        }

        public GenericResponse DeletarServico(int id)
        {
                var servicoParaDesativar = _contexto.Servico.FirstOrDefault(s => s.Id == id && s.DtFim == null);
                if (servicoParaDesativar == null)
                    return new GenericResponse { Sucesso = false, ErrorMessage = $"Serviço ativo com ID {id} não foi encontrado." };

                servicoParaDesativar.DtFim = DateTime.UtcNow;
            return MontarGenericResponse.TryExecute(() =>
            {
                _contexto.SaveChanges();
            }, "Ocorreu um erro inesperado ao desativar o serviço.");
        }
        public GenericResponse EditarServicosBarbeiro(ServicoBarbeiroEditarRequest request)
        {
            using var transaction = _contexto.Database.BeginTransaction();
            try
            {
                if (!_contexto.Barbeiro.Any(b => b.Id == request.IdBarbeiro))
                    return new GenericResponse { Sucesso = false, ErrorMessage = "Barbeiro não encontrado." };

                var servicosAtuaisAtivos = _contexto.BarbeiroServico
                    .Where(bs => bs.IdBarbeiro == request.IdBarbeiro && bs.DtFim == null)
                    .ToList();

                var idsServicosAtuais = servicosAtuaisAtivos.Select(bs => bs.IdServico).ToList();
                var idsParaAdicionar = request.IdsServico.Except(idsServicosAtuais).ToList();

                var servicosParaDesativar = servicosAtuaisAtivos
                    .Where(bs => !request.IdsServico.Contains(bs.IdServico))
                    .ToList();

                foreach (var servico in servicosParaDesativar)
                {
                    servico.DtFim = DateTime.UtcNow; 
                }

                foreach (var idServico in idsParaAdicionar)
                {
                    if (!_contexto.Servico.Any(s => s.Id == idServico))
                    {
                        transaction.Rollback(); // Desfaz a transação
                        return new GenericResponse { Sucesso = false, ErrorMessage = $"Serviço com ID {idServico} não existe." };
                    }

                    var novoServicoBarbeiro = new BarbeiroServico
                    {
                        IdBarbeiro = request.IdBarbeiro,
                        IdServico = idServico,
                        DtInicio = DateTime.UtcNow,
                        DtFim = null
                    };
                    _contexto.BarbeiroServico.Add(novoServicoBarbeiro);
                }

                _contexto.SaveChanges();
                transaction.Commit();
                return new GenericResponse { Sucesso = true};
            }
            catch
            {
                transaction.Rollback();
                return new GenericResponse { Sucesso = false, ErrorMessage = "Ocorreu um erro inesperado ao atualizar os serviços." };
            }
        }

        public List<ServicosDetalhesResponse> ListarServicosBarbeiro(int idBarbeiro)
        {
            var servicos = _contexto.BarbeiroServico
                .Where(s =>  s.IdBarbeiro == idBarbeiro && s.DtFim == null)
                .Include(x => x.Servico)
                .ThenInclude(x => x.CategoriaServico)
                .Select(s => new ServicosDetalhesResponse
                {
                    Id = s.Id,
                    Descricao = s.Servico.Descricao,
                    Valor = s.Servico.Valor,
                    Categoria = s.Servico.CategoriaServico.Descricao
                })
                .ToList();

            return servicos;
        }

        public List<ServicosDetalhesResponse> ListarServicos()
        {
            var servicos = _contexto.Servico
                .Where(s => s.DtFim == null)
                .Include(x => x.CategoriaServico)
                .Select(s => new ServicosDetalhesResponse
                {
                    Id = s.Id,
                    Descricao = s.Descricao,
                    Valor = s.Valor,
                    Categoria = s.CategoriaServico.Descricao
                })
                .ToList();

            return servicos;
        }
    }
}
