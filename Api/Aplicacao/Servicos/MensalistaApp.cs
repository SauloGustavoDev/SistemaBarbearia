using Api.Aplicacao.Contratos;
using Api.Infraestrutura.Contexto;
using Api.Modelos.Entidades;
using Api.Modelos.Enums;
using Api.Modelos.Request;
using Api.Modelos.Response;
using Microsoft.EntityFrameworkCore;

namespace Api.Aplicacao.Servicos
{
    public class MensalistaApp(Contexto contexto) : IMensalistaApp
    {
        public readonly Contexto _contexto = contexto;

        public void CadastrarMensalista(MensalistaCriarRequest request, int idBarbeiro)
        {
            Mensalista mensalista = new Mensalista(request);

            var existeMensalistaMesmoDia = _contexto.Mensalista
                .Any(m => m.Dia.DiaSemana == request.Dia.DiaSemana
                && m.Dia.Horario == request.Dia.Horario
                && m.IdBarbeiro == idBarbeiro
                && m.DtFim == null); 

            if (existeMensalistaMesmoDia)
                throw new Exception("Já existe um mensalista cadastrado para o mesmo dia e horário.");

            mensalista.IdBarbeiro = idBarbeiro;

            _contexto.Add(mensalista);
            _contexto.SaveChanges();

            RegistrarHorarios(mensalista);
        }

        private void RegistrarHorarios(Mensalista mensalista)
        {
            var ultimoDia = new DateTime(DateTime.Now.Year,DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            for (DateTime dia = DateTime.UtcNow.AddDays(1); dia <= ultimoDia; dia = dia.AddDays(1))
            {
                if(dia.DayOfWeek == mensalista.Dia.DiaSemana)
                {
                    var diaUtc = dia.ToUniversalTime();

                    var existeAgendamento = _contexto.Agendamento
                        .Where(a => a.DtAgendamento == diaUtc)
                        .Include(x => x.AgendamentoHorarios)
                            .ThenInclude(x => x.BarbeiroHorario)
                        .Where(x => x.AgendamentoHorarios
                            .Any(ah =>
                                ah.BarbeiroHorario != null &&
                                ah.BarbeiroHorario.Hora == mensalista.Dia.Horario &&
                                ah.BarbeiroHorario.IdBarbeiro == mensalista.IdBarbeiro))
                        .FirstOrDefault();

                    if (existeAgendamento == null)
                    {
                        var agendamento = new Agendamento()
                                            {
                                                IdBarbeiro = mensalista.IdBarbeiro,
                                                NomeCliente = mensalista.Nome,
                                                NumeroCliente = mensalista.Numero,
                                                DtAgendamento = dia,
                                                Status = Status.Pendente,
                                                MetodoPagamento = MetodoPagamento.Pix,
                                                AgendamentoHorarios = new List<AgendamentoHorario>
                                                {
                                                    new AgendamentoHorario
                                                    {
                                                        IdBarbeiroHorario = _contexto.BarbeiroHorario
                                                            .Where(bh => bh.IdBarbeiro == mensalista.IdBarbeiro && bh.Hora == mensalista.Dia.Horario)
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
            _contexto.SaveChanges();
        }
        public void CancelarMensalista(int id)
        {
            var mensalista = _contexto.Mensalista.Find(id) ?? throw new Exception("Mensalista não encontrado");
            mensalista.DtFim = DateTime.Now;
            mensalista.Status = StatusMensalista.Cancelado;
            _contexto.SaveChanges();

        }
        public List<MensalistaDetalhesResponse> ListarMensalistas(int idBarbeiro)
        {
           var mensalistas = _contexto.Mensalista
                .Include(m => m.Dia)
                .Where(m => m.IdBarbeiro == idBarbeiro && m.DtFim == null)
                .ToList();

            return mensalistas.Select(m => new MensalistaDetalhesResponse(m)).ToList();
        }  
    }
}
