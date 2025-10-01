using Api.Aplicacao.Contratos;
using Api.Infraestrutura.Contexto;
using Api.Modelos.Enums;
using Microsoft.EntityFrameworkCore;

namespace Api.Aplicacao.Servicos
{
    public class Worker(Contexto contexto, ILogger<Worker> logger) : IWorker
    {
        private readonly ILogger<Worker> _logger = logger;
        public readonly Contexto _contexto = contexto;

        public async Task EnviarLembreteAgendamentos()
        {
            _logger.LogInformation("Worker: Iniciando verificação de lembretes de agendamento.");

            try
            {
                var agora = DateTime.UtcNow;
                var umaHoraDepois = agora.AddHours(1);

                // Busca agendamentos confirmados que estão para acontecer na próxima hora
                // e que ainda não tiveram lembrete enviado (assumindo uma flag \'LembreteEnviado\' no Agendamento)
                var agendamento = await _contexto.Agendamento
                    .Where(a => a.Status == Status.Pendente &&
                                a.DtAgendamento.ToUniversalTime() > agora &&
                                a.DtAgendamento.ToUniversalTime() <= umaHoraDepois)
                    .FirstOrDefaultAsync();

                if (agendamento == null)
                    return; // Nenhum agendamento para lembrar

                var mesmoAgendamentoSeguinte = await _contexto.Agendamento
                    .Where(a => a.NumeroCliente == agendamento.NumeroCliente &&
                    a.DtAgendamento.Date.ToUniversalTime() == agora.Date.ToUniversalTime()
                    && a.Status == Status.Pendente)
                    .ToListAsync();

                agendamento.Status = Status.LembreteEnviado;
                mesmoAgendamentoSeguinte.ForEach(x => x.Status = Status.LembreteEnviado);
                _logger.LogInformation($"Worker: Enviando lembrete de agendamento! Cliente: {agendamento.NomeCliente} - Numero: {agendamento.NumeroCliente}.");

                // Supondo que você tenha o número de telefone do cliente no modelo Agendamento
                // e que o IWhatsAppService tenha um método para enviar o lembrete.
                //await _whatsAppService.EnviarMensagemLembrete(
                //    agendamento.Id,
                //    agendamento.NomeCliente, // Ou outro identificador do cliente
                //    agendamento.NumeroTelefoneCliente, // Este campo precisa existir no seu modelo Agendamento
                //    agendamento.DtAgendamento.ToLocalTime().ToString("HH:mm")
                //);


                await _contexto.SaveChangesAsync();
                _logger.LogInformation($"Worker: Lembrete de agendamento enviado.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Worker: Erro ao enviar lembretes de agendamento.");
            }
        }
    }
}
