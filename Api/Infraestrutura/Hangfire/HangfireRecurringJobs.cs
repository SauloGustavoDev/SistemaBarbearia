using Api.Aplicacao.Contratos;
using Hangfire;

namespace Api.Infraestrutura.Hangfire
{
    public static class HangfireRecurringJobs
    {
        public static IApplicationBuilder UseHangfireRecurringJobs(this IApplicationBuilder app)
        {
            // Substitui Cron.MinuteInterval(10) por a expressão cron equivalente: "*/10 * * * *"
            RecurringJob.AddOrUpdate<IWorker>(
                "enviar-notificacao-corte",
                worker => worker.EnviarNotificacaoCorte(),
                "*/10 * * * *"
            );

            RecurringJob.AddOrUpdate<IWorker>(
                "Agendamentos-Mensalistas",
                worker => worker.GerarAgendamentosMensalistas(),
                "0 0 1 * *"
            );

            return app;
        }
    }
}
