using Api.Models.Entity;

namespace Api.Aplicacao.Contratos
{
    public interface IBarbeiroApp
    {
        void Cadastrar(Barbeiro barbeiro);
        void Editar(Barbeiro barbeiro);
        void Excluir(int id);
    }
}
