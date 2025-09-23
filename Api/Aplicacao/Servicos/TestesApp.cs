using Api.Aplicacao.Contratos;
using Api.Aplicacao.Helpers;
using Api.Infraestrutura.Contexto;
using Api.Modelos.Dtos;
using Api.Modelos.Entidades;
using Api.Modelos.Enums;
using Api.Modelos.Response;
using Microsoft.EntityFrameworkCore;

namespace Api.Aplicacao.Servicos
{
    public class TestesApp(Contexto contexto) : ITestesApp
    {
        public readonly Contexto _contexto = contexto;

        public async Task<GenericResponse> GerarBancoSimulado()
        {
            // Envolvemos tudo em uma transação para garantir que ou tudo é salvo, ou nada é.
            await using var transaction = await _contexto.Database.BeginTransactionAsync();
            try
            {
                // --- 1. SEED DE BARBEIROS (com verificação de existência) ---
                var emailsBarbeirosExistentes = await _contexto.Barbeiro
                    .Select(b => b.Email)
                    .ToHashSetAsync();

                var barbeirosParaAdicionar = new List<BarbeiroCriarRequest>
        {
            new() { Nome = "Carlos Almeida", Numero = "11987654321", Email = "carlos.almeida@barbearia.dev", Acesso = Acesso.Barbeiro, Descricao = "Especialista em cortes clássicos e barba. Na casa há 5 anos.", Senha = Criptografia.GerarSenha("senha_forte_123") },
            new() { Nome = "Bruno Santos", Numero = "21912345678", Email = "bruno.santos@barbearia.dev", Acesso = Acesso.Barbeiro, Descricao = "Foco em cortes modernos, degradê e navalhado. Sempre antenado nas novas tendências.", Senha = Criptografia.GerarSenha("senha_forte_123") },
            new() { Nome = "Ricardo Lima", Numero = "31955558888", Email = "ricardo.lima@barbearia.dev", Acesso = Acesso.Admin, Descricao = "Gerente e barbeiro mais experiente. Mestre em todas as técnicas de corte e barba.", Senha = Criptografia.GerarSenha("senha_forte_123") }
        }
                .Where(req => !emailsBarbeirosExistentes.Contains(req.Email))
                .Select(req => new Barbeiro(req))
                .ToList();

                if (barbeirosParaAdicionar.Count != 0)
                    await _contexto.Barbeiro.AddRangeAsync(barbeirosParaAdicionar);

                // --- 2. SEED DE CATEGORIAS (com verificação de existência) ---
                var nomesCategoriasExistentes = await _contexto.CategoriaServico
                    .Select(c => c.Descricao)
                    .ToHashSetAsync();

                var categoriasParaAdicionar = new List<CategoriaServico>
        {
            new() { Descricao = "Cabelo" },
            new() { Descricao = "Barba" },
            new() { Descricao = "Bigode" },
            new() { Descricao = "Sobrancelha" }
        }
                .Where(cat => !nomesCategoriasExistentes.Contains(cat.Descricao))
                .ToList();

                if (categoriasParaAdicionar.Count != 0)
                    await _contexto.CategoriaServico.AddRangeAsync(categoriasParaAdicionar);

                // --- Salva barbeiros e categorias para gerar IDs ---
                await _contexto.SaveChangesAsync();

                // --- 3. SEED DE SERVIÇOS ---
                var todasAsCategorias = await _contexto.CategoriaServico
                .Where(c => c.Descricao != null)
                .ToDictionaryAsync(c => c.Descricao!, c => c.Id);

                var nomesServicosExistentes = await _contexto.Servico
                    .Select(s => s.Descricao)
                    .ToHashSetAsync();

                var servicosParaAdicionar = new List<Servico>
        {
            new() { Descricao = "Degrade", Valor = 35, TempoEstimado = new TimeOnly(0, 40, 0), IdCategoriaServico = todasAsCategorias["Cabelo"] },
            new() { Descricao = "Social", Valor = 35, TempoEstimado = new TimeOnly(0, 40, 0), IdCategoriaServico = todasAsCategorias["Cabelo"] },
            new() { Descricao = "Navalhado", Valor = 40, TempoEstimado = new TimeOnly(0, 40, 0), IdCategoriaServico = todasAsCategorias["Cabelo"] },
            new() { Descricao = "Shaver", Valor = 40, TempoEstimado = new TimeOnly(0, 40, 0), IdCategoriaServico = todasAsCategorias["Cabelo"] },
            new() { Descricao = "Luzes", Valor = 150, TempoEstimado = new TimeOnly(1, 0, 0), IdCategoriaServico = todasAsCategorias["Cabelo"] },
            new() { Descricao = "Platinado", Valor = 150, TempoEstimado = new TimeOnly(1, 0, 0), IdCategoriaServico = todasAsCategorias["Cabelo"] },
            new() { Descricao = "Progressiva", Valor = 120, TempoEstimado = new TimeOnly(1, 0, 0), IdCategoriaServico = todasAsCategorias["Cabelo"] },
            new() { Descricao = "Hidratação", Valor = 25, TempoEstimado = new TimeOnly(0, 40, 0), IdCategoriaServico = todasAsCategorias["Cabelo"] },
            new() { Descricao = "Alisamento", Valor = 30, TempoEstimado = new TimeOnly(0, 40, 0), IdCategoriaServico = todasAsCategorias["Cabelo"] },
            new() { Descricao = "Penteado", Valor = 20, TempoEstimado = new TimeOnly(0, 40, 0), IdCategoriaServico = todasAsCategorias["Cabelo"] },
            new() { Descricao = "Sobrancelha", Valor = 15, TempoEstimado = new TimeOnly(0, 10, 0), IdCategoriaServico = todasAsCategorias["Sobrancelha"] },
            new() { Descricao = "Barba", Valor = 30, TempoEstimado = new TimeOnly(0, 40, 0), IdCategoriaServico = todasAsCategorias["Barba"] }
        }
                .Where(s => !nomesServicosExistentes.Contains(s.Descricao))
                .ToList();

                servicosParaAdicionar.ForEach(s => s.DtInicio = DateTime.UtcNow);

                if (servicosParaAdicionar.Count != 0)
                    await _contexto.Servico.AddRangeAsync(servicosParaAdicionar);

                await _contexto.SaveChangesAsync();

                await transaction.CommitAsync();
                return new GenericResponse { Sucesso = true };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new GenericResponse { Sucesso = false, ErrorMessage = $"Ocorreu um erro inesperado: {ex.Message}" };
            }
        }


        public async Task<GenericResponse> LimparBancoDeDados()
        {
            return await MontarGenericResponse.TryExecuteAsync(async () =>
            {
                _contexto.AgendamentoServico.RemoveRange(_contexto.AgendamentoServico);
                _contexto.AgendamentoHorario.RemoveRange(_contexto.AgendamentoHorario);
                _contexto.BarbeiroHorarioExcecao.RemoveRange(_contexto.BarbeiroHorarioExcecao);
                _contexto.BarbeiroServico.RemoveRange(_contexto.BarbeiroServico);
                await _contexto.SaveChangesAsync();
                _contexto.Agendamento.RemoveRange(_contexto.Agendamento);
                _contexto.BarbeiroHorario.RemoveRange(_contexto.BarbeiroHorario);
                await _contexto.SaveChangesAsync();
                _contexto.Servico.RemoveRange(_contexto.Servico);
                _contexto.CategoriaServico.RemoveRange(_contexto.CategoriaServico);
                await _contexto.SaveChangesAsync();
                _contexto.Barbeiro.RemoveRange(_contexto.Barbeiro);
                await _contexto.SaveChangesAsync();
            }, "Erro ao limpar banco de dados.");
        }
    }
}
