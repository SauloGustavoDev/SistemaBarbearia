using Api.Aplicacao.Contratos;
using Api.Infraestrutura.Contexto;
using Api.Modelos.Request;
using Api.Modelos.Response;
using Microsoft.EntityFrameworkCore;

namespace Api.Aplicacao.Servicos
{
    public class RelatorioApp(Contexto contexto) : IRelatorioApp
    {
        private readonly Contexto _contexto = contexto;

        public RelatorioFinanceiro GerarRelatorioFinanceiro(RelatorioFinanceiroRequest request)
        {
            request.DataInicio = request.DataInicio.HasValue ? request.DataInicio : DateTime.Now.Date.ToUniversalTime();
            request.DataFim = request.DataFim.HasValue ? request.DataFim : DateTime.Now.Date.ToUniversalTime();

            var agendamentos = _contexto.Agendamento
                .Include(x => x.AgendamentoServicos)
                .ThenInclude(x => x.Servico)
                .Where(a => a.IdBarbeiro == request.IdBarbeiro &&
                            a.DtAgendamento.ToUniversalTime() >= request.DataInicio.Value.ToUniversalTime() &&
                            a.DtAgendamento.ToUniversalTime() <= request.DataFim.Value.ToUniversalTime() &&
                            a.Status == Modelos.Enums.Status.Concluido)
                .ToList();

            var barbeiro = _contexto.Barbeiro.Find(request.IdBarbeiro) ?? throw new Exception("Barbeiro não encontrado");

            if (agendamentos.Count == 0)
                return new RelatorioFinanceiro { Barbeiro = barbeiro.Nome, ValorArrecadado = 0, CortesRealizados = 0 };

            var relatorio = new RelatorioFinanceiro
            {
                Barbeiro = barbeiro.Nome,
                CortesRealizados = agendamentos.Count,
                ValorArrecadado = agendamentos.Sum(a => a.AgendamentoServicos
                    .Where(s => s.Servico != null)
                    .Sum(s => s.Servico!.Valor))
            };

            var top10Clientes = agendamentos
                .GroupBy(a => new { a.NumeroCliente, a.NomeCliente })
                .OrderByDescending(g => g.Count())
                .Take(10)                               // pega só os 10 primeiros
                .Select(a => new RelatorioTop10Clientes
                {
                    Nome = a.Key.NomeCliente,
                    Numero = a.Key.NumeroCliente,
                    TotalCortes = a.Count(),
                    UltimoCorte = a.Max(x => x.DtAgendamento)
                });

            var top10Servicos = agendamentos
              .SelectMany(a => a.AgendamentoServicos)
              .Where(s => s.Servico != null)
              .GroupBy(s => new { s.Servico!.Id, s.Servico.Descricao }) // agrupa por ID e nome
              .OrderByDescending(g => g.Count())
              .Take(10)
              .Select(g => new RelatorioTop10Servicos
              {
                  Servico = g.Key.Descricao,
                  TotalRealizados = g.Count()
              })
              .ToList();

            relatorio.Top10Clientes = top10Clientes.ToList();
            relatorio.Top10Servicos = top10Servicos;

            return relatorio;
        }
    }
}
