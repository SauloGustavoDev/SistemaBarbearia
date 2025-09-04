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

            var horariosSemana = DiasSemanas();
            var sabado = DiaSabado();

            foreach (var dia in horariosSemana)
            {
                BarbeiroHorarios.Add(new BarbeiroHorario {Hora = dia, TipoDia = (TipoDia)1, DtInicio = DateTime.UtcNow, DtFim = null  });
            }
            foreach (var dia in sabado)
            {
                BarbeiroHorarios.Add(new BarbeiroHorario { Hora = dia, TipoDia = (TipoDia)2, DtInicio = DateTime.UtcNow, DtFim = null });
            }

        }

        private List<TimeOnly> DiasSemanas()
        {
            return new List<TimeOnly>
            {
                new TimeOnly(10, 0),
                new TimeOnly(10, 40),
                new TimeOnly(11, 20),
                new TimeOnly(12, 0),
                new TimeOnly(13, 20),
                new TimeOnly(14, 0),
                new TimeOnly(14, 40),
                new TimeOnly(15, 20),
                new TimeOnly(16, 0),
                new TimeOnly(16, 40),
                new TimeOnly(17, 20),
                new TimeOnly(18, 0),
                new TimeOnly(18, 40),
                new TimeOnly(19, 20),
                new TimeOnly(20, 0)
            };
        }

        private List<TimeOnly> DiaSabado()
        {
            return new List<TimeOnly>
            {
                new TimeOnly(9, 0),
                new TimeOnly(9, 40),
                new TimeOnly(10, 20),
                new TimeOnly(11, 0),
                new TimeOnly(11, 40),
                new TimeOnly(12, 20),
                new TimeOnly(13, 20),
                new TimeOnly(14, 0),
                new TimeOnly(14, 40),
                new TimeOnly(15, 20),
                new TimeOnly(16, 0),
                new TimeOnly(16, 40),
                new TimeOnly(17, 20),
                new TimeOnly(18, 0),
                new TimeOnly(18, 40),
                new TimeOnly(19, 0)
            };
        }
    }
}
