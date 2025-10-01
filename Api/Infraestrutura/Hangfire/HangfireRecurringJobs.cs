using Api.Aplicacao.Contratos;
using Hangfire;

namespace Api.Infraestrutura.Hangfire
{
    public static class HangfireRecurringJobs
    {
        public static IApplicationBuilder UseHangfireRecurringJobs(this IApplicationBuilder app)
        {

            RecurringJob.AddOrUpdate<IWorker>("enviar-lembretes-agendamento", worker => worker.EnviarLembreteAgendamentos(), "*/40 * * * * 9,20 * * * 1-5");

            return app;
        }
    }
}
