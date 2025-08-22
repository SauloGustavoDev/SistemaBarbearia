// Api.Modelos.Entidades/Agendamento.cs

using Api.Modelos.Enums;
using Api.Modelos.Request;
using Api.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Modelos.Entidades
{
    public class Agendamento
    {
        public int Id { get; set; }
        public int IdBarbeiro { get; set; }
        public string NomeCliente { get; set; }
        public string NumeroCliente { get; set; }
        public DateTime DtAgendamento { get; set; }
        public Status Status { get; set; }

        // Navegações
        public Barbeiro Barbeiro { get; set; }
        public List<AgendamentoHorario> AgendamentoHorarios { get; private set; } = new List<AgendamentoHorario>();
        public List<AgendamentoServico> AgendamentoServicos { get; private set; } = new List<AgendamentoServico>();

        // Construtor para o EF Core
        public Agendamento() { }

        // Construtor a partir do DTO (Request)
        public Agendamento(AgendamentoCriarRequest request)
        {
            // 1. Mapeia as propriedades diretas
            IdBarbeiro = request.IdBarbeiro;
            NomeCliente = request.Nome;
            NumeroCliente = request.Numero;
            DtAgendamento = request.DtAgendamento.Date; // Armazena apenas a data, sem a hora
            Status = Status.Pendente; // Define um status inicial padrão

            // 2. Popula a lista de junção AgendamentoHorarios
            // Para cada Id de horário recebido, cria uma nova entidade de junção
            AgendamentoHorarios = request.IdsHorario.Select(idHorario => new AgendamentoHorario
            {
                IdBarbeiroHorario = idHorario,
                // O IdAgendamento será preenchido pelo EF Core quando o Agendamento for salvo.
            }).ToList();

            // 3. Popula a lista de junção AgendamentoServicos
            // Para cada Id de serviço recebido, cria uma nova entidade de junção
            AgendamentoServicos = request.IdsServico.Select(idServico => new AgendamentoServico
            {
                IdServico = idServico
                // O IdAgendamento também será preenchido pelo EF Core.
            }).ToList();
        }
    }
}
