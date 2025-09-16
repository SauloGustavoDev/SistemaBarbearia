using Api.Aplicacao.Contratos;
using Api.Aplicacao.Helpers;
using Api.Infraestrutura.Contexto;
using Api.Modelos.Dtos;
using Api.Modelos.Entidades;
using Api.Modelos.Enums;
using Api.Modelos.Response;
using Api.Models.Entity;

namespace Api.Aplicacao.Servicos
{
    public class TestesApp : ITestesApp
    {
        public readonly Contexto _contexto;
        public TestesApp(Contexto contexto)
        {
            _contexto = contexto;
        }

        public GenericResponse GerarBancoSimulado()
        {
            // Envolvemos tudo em uma transação para garantir que ou tudo é salvo, ou nada é.
            using var transaction = _contexto.Database.BeginTransaction();
            try
            {
                // --- 1. SEED DE BARBEIROS (com verificação de existência) ---
                var emailsBarbeirosExistentes = _contexto.Barbeiro.Select(b => b.Email).ToHashSet();
                var barbeirosParaAdicionar = new List<BarbeiroCriarRequest>
                {
                new BarbeiroCriarRequest{ Nome = "Carlos Almeida",Numero = "11987654321",Email = "carlos.almeida@barbearia.dev",Acesso = Acesso.Barbeiro, Descricao = "Especialista em cortes clássicos e barba. Na casa há 5 anos.",Senha = Criptografia.GerarSenha("senha_forte_123") },
                new BarbeiroCriarRequest{Nome = "Bruno Santos",Numero = "21912345678",Email = "bruno.santos@barbearia.dev",Acesso = Acesso.Barbeiro,Descricao = "Foco em cortes modernos, degradê e navalhado. Sempre antenado nas novas tendências.",Senha = Criptografia.GerarSenha("senha_forte_123")},
                new BarbeiroCriarRequest{Nome = "Ricardo Lima",Numero = "31955558888",Email = "ricardo.lima@barbearia.dev",Acesso = Acesso.Admin, Descricao = "Gerente e barbeiro mais experiente. Mestre em todas as técnicas de corte e barba.",Senha = Criptografia.GerarSenha("senha_forte_123")}
                }
                .Where(req => !emailsBarbeirosExistentes.Contains(req.Email)) // Filtra apenas os que não existem
                .Select(req => new Barbeiro(req)) // Converte para a entidade
                .ToList();

                if (barbeirosParaAdicionar.Any())
                {
                    _contexto.Barbeiro.AddRange(barbeirosParaAdicionar);
                }

                // --- 2. SEED DE CATEGORIAS (com verificação de existência) ---
                var nomesCategoriasExistentes = _contexto.CategoriaServico.Select(c => c.Descricao).ToHashSet();
                var categoriasParaAdicionar = new List<CategoriaServico>
        {
            new CategoriaServico { Descricao = "Cabelo" }, // Usando 'Nome' para consistência
            new CategoriaServico { Descricao = "Barba" },
            new CategoriaServico { Descricao = "Bigode" },
            new CategoriaServico { Descricao = "Sobrancelha" }
        }
                .Where(cat => !nomesCategoriasExistentes.Contains(cat.Descricao))
                .ToList();

                if (categoriasParaAdicionar.Any())
                {
                    _contexto.CategoriaServico.AddRange(categoriasParaAdicionar);
                }

                // --- IMPORTANTE: Salva barbeiros e categorias para que seus IDs sejam gerados pelo banco ---
                _contexto.SaveChanges();

                // --- 3. SEED DE SERVIÇOS (com verificação e obtenção de IDs corretos) ---
                // Busca todas as categorias (as antigas + as novas) para ter um mapa de Nome -> ID.
                var todasAsCategorias = _contexto.CategoriaServico.ToDictionary(c => c.Descricao, c => c.Id);
                var nomesServicosExistentes = _contexto.Servico.Select(s => s.Descricao).ToHashSet();

                var servicosParaAdicionar = new List<Servico>
        {
            new Servico { Descricao = "Degrade", Valor = 35, TempoEstimado = new TimeOnly(0, 40, 0), IdCategoriaServico = todasAsCategorias["Cabelo"] },
            new Servico { Descricao = "Social", Valor = 35, TempoEstimado = new TimeOnly(0, 40, 0), IdCategoriaServico = todasAsCategorias["Cabelo"] },
            new Servico { Descricao = "Navalhado", Valor = 40, TempoEstimado = new TimeOnly(0, 40, 0), IdCategoriaServico = todasAsCategorias["Cabelo"] },
            new Servico { Descricao = "Shaver", Valor = 40, TempoEstimado = new TimeOnly(0, 40, 0), IdCategoriaServico = todasAsCategorias["Cabelo"] },
            new Servico { Descricao = "Luzes", Valor = 150, TempoEstimado = new TimeOnly(1, 0, 0), IdCategoriaServico = todasAsCategorias["Cabelo"] },
            new Servico { Descricao = "Platinado", Valor = 150, TempoEstimado = new TimeOnly(1, 0, 0), IdCategoriaServico = todasAsCategorias["Cabelo"] },
            new Servico { Descricao = "Progressiva", Valor = 120, TempoEstimado = new TimeOnly(1, 0, 0), IdCategoriaServico = todasAsCategorias["Cabelo"] },
            new Servico { Descricao = "Hidratação", Valor = 25, TempoEstimado = new TimeOnly(0, 40, 0), IdCategoriaServico = todasAsCategorias["Cabelo"] },
            new Servico { Descricao = "Alisamento", Valor = 30, TempoEstimado = new TimeOnly(0, 40, 0), IdCategoriaServico = todasAsCategorias["Cabelo"] },
            new Servico { Descricao = "Penteado", Valor = 20, TempoEstimado = new TimeOnly(0, 40, 0), IdCategoriaServico = todasAsCategorias["Cabelo"] },
            new Servico { Descricao = "Sobrancelha", Valor = 15, TempoEstimado = new TimeOnly(0, 10, 0), IdCategoriaServico = todasAsCategorias["Sobrancelha"] }, // Corrigido o valor e tempo
            new Servico { Descricao = "Barba", Valor = 30, TempoEstimado = new TimeOnly(0, 40, 0), IdCategoriaServico = todasAsCategorias["Barba"] }
        }
                .Where(s => !nomesServicosExistentes.Contains(s.Descricao))
                .ToList();

                // Adiciona a data de início apenas para os novos serviços
                servicosParaAdicionar.ForEach(s => s.DtInicio = DateTime.UtcNow);

                if (servicosParaAdicionar.Any())
                {
                    _contexto.Servico.AddRange(servicosParaAdicionar);
                    _contexto.SaveChanges(); // Salva os serviços
                }

                transaction.Commit(); // Confirma todas as operações
                return new GenericResponse { Sucesso = true };
            }
            catch (Exception ex)
            {
                transaction.Rollback(); // Desfaz tudo em caso de erro
                                        // Logar a exceção 'ex' aqui
                return new GenericResponse { Sucesso = false, ErrorMessage = $"Ocorreu um erro inesperado: {ex.Message}" };
            }
        }


        public GenericResponse LimparBancoDeDados()
        {

            return MontarGenericResponse.TryExecute(() =>
            {
                _contexto.AgendamentoServico.RemoveRange(_contexto.AgendamentoServico);
                _contexto.AgendamentoHorario.RemoveRange(_contexto.AgendamentoHorario);
                _contexto.BarbeiroHorarioExcecao.RemoveRange(_contexto.BarbeiroHorarioExcecao);
                _contexto.BarbeiroServico.RemoveRange(_contexto.BarbeiroServico);
                _contexto.SaveChanges();
                _contexto.Agendamento.RemoveRange(_contexto.Agendamento);
                _contexto.BarbeiroHorario.RemoveRange(_contexto.BarbeiroHorario);
                _contexto.SaveChanges();
                _contexto.Servico.RemoveRange(_contexto.Servico);
                _contexto.CategoriaServico.RemoveRange(_contexto.CategoriaServico);
                _contexto.SaveChanges();
                _contexto.Barbeiro.RemoveRange(_contexto.Barbeiro);
                _contexto.SaveChanges();
            }, "Erro ao limpar banco de dados.");
        }
    }
}
