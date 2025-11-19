namespace Api.Aplicacao.Contratos
{
    public interface IWorker
    {
        void EnviarNotificacaoCorte();
        void GerarAgendamentosMensalistas();
    }
}
