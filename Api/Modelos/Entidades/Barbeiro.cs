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
        public byte[] Foto { get; set; }
        public Acesso Acesso { get; set; }
        public string Senha { get; set; }
        public string Descricao { get; set; }
        public DateTime DtCadastro { get; set; }
        public DateTime DtDemissao { get; set; }
        public List<Servico> Servicos { get; set; }
        public List<BarbeiroHorario> BarbeiroHorario{ get; set; }
        public List<Agendamento> Agendamentos { get; set; }
        public Barbeiro()
        {
            
        }
        public Barbeiro(BarbeiroDto barbeiroDto)
        {
            Nome = barbeiroDto.Nome;
            Numero = barbeiroDto.Numero;
            Acesso = barbeiroDto.Acesso;
            Descricao = barbeiroDto.Descricao;
            Senha = barbeiroDto.Senha;
        }
    }
}
