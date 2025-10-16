using Api.Aplicacao.Contratos;
using Hangfire;

namespace Api.Infraestrutura.Hangfire
{
    public static class HangfireRecurringJobs
    {
        public static IApplicationBuilder UseHangfireRecurringJobs(this IApplicationBuilder app)
        {

            RecurringJob.AddOrUpdate<IWorker>("enviar-lembretes-agendamento", worker => worker.EnviarLembreteAgendamentos(), "");

            return app;
        }
    }
}
