using Api.Modelos.Dtos;
using Api.Modelos.Entidades;
using Api.Modelos.Enums;

namespace Api.Models.Entity
{
    public class Barbeiro
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Numero { get; set; }
        public string Email { get; set; }
        public byte[]? Foto { get; set; }
        public Acesso Acesso { get; set; }
        public string Senha { get; set; }
        public string? Descricao { get; set; }
        public DateTimeOffset DtCadastro { get; set; }
        public DateTime? DtDemissao { get; set; }
        public List<BarbeiroServico>? BarbeiroServicos { get; set; } = new List<BarbeiroServico>();
        public List<BarbeiroHorario>? BarbeiroHorarios{ get; set; } = new List<BarbeiroHorario>();
        public List<Agendamento>? Agendamentos { get; set; } = new List<Agendamento>();
        public Barbeiro()
        {
            
        }
        public Barbeiro(BarbeiroCriarRequest barbeiroDto)
        {
            Nome = barbeiroDto.Nome;
            Numero = barbeiroDto.Numero;
            Acesso = barbeiroDto.Acesso;
            Descricao = barbeiroDto.Descricao;
            Senha = barbeiroDto.Senha;
            Email = barbeiroDto.Email;
            DtCadastro = DateTimeOffset.Now.ToUniversalTime();

            var horariosSemana = MontarHorarioDiaSemana();
            var sabado = MontarHorarioSabado();

            foreach (var dia in horariosSemana)
            {
                BarbeiroHorarios.Add(new BarbeiroHorario {Hora = dia, TipoDia = TipoDia.SegundaASexta, DtInicio = DateTime.UtcNow, DtFim = null  });
            }
            foreach (var dia in sabado)
            {
                BarbeiroHorarios.Add(new BarbeiroHorario { Hora = dia, TipoDia = TipoDia.Sabado, DtInicio = DateTime.UtcNow, DtFim = null });
            }

        }

        private List<TimeOnly> MontarHorarioDiaSemana()
        {
            var horarios = new List<TimeOnly>();
            var horaManha = TimeOnly.Parse("10:00");
            horarios.Add(horaManha);
            while (horaManha < TimeOnly.Parse("12:00"))
            {
                horaManha = horaManha.AddMinutes(40);
                horarios.Add(horaManha);
            }
            var horaTarde = TimeOnly.Parse("13:20");
            horarios.Add(horaTarde);
            while (horaTarde < TimeOnly.Parse("20:00"))
            {
                horaTarde = horaTarde.AddMinutes(40);
                horarios.Add(horaTarde);
            }
            return horarios;
        }

        private List<TimeOnly> MontarHorarioSabado()
        {
            var horarios = new List<TimeOnly>();
            var horaManha = TimeOnly.Parse("09:00");
            horarios.Add(horaManha);
            while (horaManha < TimeOnly.Parse("12:20"))
            {
                horaManha = horaManha.AddMinutes(40);
                horarios.Add(horaManha);
            }
            var horaTarde = TimeOnly.Parse("13:20");
            horarios.Add(horaTarde);
            while (horaTarde < TimeOnly.Parse("19:00"))
            {
                horaTarde = horaTarde.AddMinutes(40);
                horarios.Add(horaTarde);
            }
            return horarios;
        }
    }
}
