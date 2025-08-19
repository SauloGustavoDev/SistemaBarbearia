using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Api.Infraestrutura.Contexto
{
    public class SqlContextoFactory : IDesignTimeDbContextFactory<Contexto>
    {
        public Contexto CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Contexto>();

            // 🔹 Use PostgreSQL em vez de SQLite
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=barbearia_db;Username=postgres;Password=suasenha");

            return new Contexto(optionsBuilder.Options);
        }

    }
}
