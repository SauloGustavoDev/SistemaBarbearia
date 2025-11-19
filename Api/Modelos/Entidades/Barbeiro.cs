using Api.Aplicacao.Helpers;
using Api.Modelos.Dtos;
using Api.Modelos.Entidades;
using Api.Modelos.Enums;

namespace Api.Modelos.Entidades
{
    public class Barbeiro
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[]? Foto { get; set; }
        public Acesso Acesso { get; set; }
        public string Senha { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public TipoAgenda Agenda { get; set; }
        public DateTimeOffset DtCadastro { get; set; }
        public DateTime? DtDemissao { get; set; }
        public List<BarbeiroServico>? BarbeiroServicos { get; set; } = [];
        public List<BarbeiroHorario>? BarbeiroHorarios { get; set; } = [];
        public List<Agendamento>? Agendamentos { get; set; } = [];
        public List<Mensalista>? Mensalistas { get; set; } = [];
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

            var horariosSemana = HelperGenerico.MontarHorarioDiaSemana();
            var sabado = HelperGenerico.MontarHorarioSabado();

            foreach (var dia in horariosSemana)
            {
                BarbeiroHorarios.Add(new BarbeiroHorario { Hora = dia, TipoDia = TipoDia.SegundaASexta, DtInicio = DateTime.UtcNow, DtFim = null });
            }
            foreach (var dia in sabado)
            {
                BarbeiroHorarios.Add(new BarbeiroHorario { Hora = dia, TipoDia = TipoDia.Sabado, DtInicio = DateTime.UtcNow, DtFim = null });
            }

        }

     
    }
}
