using Api.Aplicacao.Contratos;
using Api.Infraestrutura.Contexto;
using Api.Migrations;
using Api.Modelos.Entidades;
using Api.Modelos.Enums;
using Microsoft.EntityFrameworkCore;

namespace Api.Aplicacao.Servicos
{
    public class Worker(Contexto contexto, ILogger<Worker> logger) : IWorker
    {
        private readonly ILogger<Worker> _logger = logger;
        public readonly Contexto _contexto = contexto;

        public void EnviarNotificacaoCorte()
        {
            _logger.LogInformation("Worker: Iniciando verificação de lembretes de agendamento.");

            try
            {
                var agora = DateTime.UtcNow;
                var umaHoraDepois = agora.AddHours(1);
                var agendamento =  _contexto.Agendamento
                    .Where(a => a.Status == Status.Pendente &&
                                a.DtAgendamento.ToUniversalTime() == umaHoraDepois)
                    .FirstOrDefault();

                if (agendamento == null)
                    return; // Nenhum agendamento para lembrar


                _logger.LogInformation($"Worker: Enviando lembrete de agendamento! Cliente: {agendamento.NomeCliente} - Numero: {agendamento.NumeroCliente}.");

                // Supondo que você tenha o número de telefone do cliente no modelo Agendamento
                // e que o IWhatsAppService tenha um método para enviar o lembrete.
                //await _whatsAppService.EnviarMensagemLembrete(
                //    agendamento.Id,
                //    agendamento.NomeCliente, // Ou outro identificador do cliente
                //    agendamento.NumeroTelefoneCliente, // Este campo precisa existir no seu modelo Agendamento
                //    agendamento.DtAgendamento.ToLocalTime().ToString("HH:mm")
                //);

                agendamento.Status = Status.LembreteEnviado;

                _contexto.SaveChanges();
                _logger.LogInformation($"Worker: Lembrete de agendamento enviado.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Worker: Erro ao enviar lembretes de agendamento.");
            }
        }
        public void GerarAgendamentosMensalistas()
        {
            var ultimoDia = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            var mensalistas = _contexto.Mensalista
                .Where(m => m.DtFim == null && m.Tipo == TipoMensalista.Mensal && m.Status == StatusMensalista.RenovacaoAutomatica)
                .ToList();

            foreach (var mensal in mensalistas)
            {
                for (DateTime dia = DateTime.UtcNow.AddDays(1); dia <= ultimoDia; dia = dia.AddDays(1))
                {
                    if (dia.DayOfWeek == mensal.Dia.DiaSemana)
                    {
                        var diaUtc = dia.ToUniversalTime();

                        var existeAgendamento = _contexto.Agendamento
                            .Where(a => a.DtAgendamento == diaUtc)
                            .Include(x => x.AgendamentoHorarios)
                                .ThenInclude(x => x.BarbeiroHorario)
                            .Where(x => x.AgendamentoHorarios
                                .Any(ah =>
                                    ah.BarbeiroHorario != null &&
                                    ah.BarbeiroHorario.Hora == mensal.Dia.Horario &&
                                    ah.BarbeiroHorario.IdBarbeiro == mensal.IdBarbeiro))
                            .FirstOrDefault();

                        if (existeAgendamento == null)
                        {
                            var agendamento = new Agendamento()
                            {
                                IdBarbeiro = mensal.IdBarbeiro,
                                NomeCliente = mensal.Nome,
                                NumeroCliente = mensal.Numero,
                                DtAgendamento = dia,
                                Status = Status.Pendente,
                                MetodoPagamento = MetodoPagamento.Pix,
                                AgendamentoHorarios = new List<AgendamentoHorario>
                                                {
                                                    new AgendamentoHorario
                                                    {
                                                        IdBarbeiroHorario = _contexto.BarbeiroHorario
                                                            .Where(bh => bh.IdBarbeiro == mensal.IdBarbeiro && bh.Hora == mensal.Dia.Horario)
                                                            .Select(bh => bh.Id)
                                                            .FirstOrDefault()
                                                    }
                                                },
                                AgendamentoServicos = new List<AgendamentoServico>
                                                {
                                                    new AgendamentoServico
                                                    {
                                                        IdServico = _contexto.Servico
                                                        .Include(s => s.CategoriaServico)
                                                            .Where(s => s.CategoriaServico != null && s.CategoriaServico.Descricao == "Mensalista")
                                                            .Select(s => s.Id)
                                                            .FirstOrDefault()
                                                    }
                                                }
                            };
                            _contexto.Agendamento.Add(agendamento);
                        }

                    }
                }
            }
           
            _contexto.SaveChanges();
        }
    }
}
