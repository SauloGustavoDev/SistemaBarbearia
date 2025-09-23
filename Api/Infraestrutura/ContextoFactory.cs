using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Api.Infraestrutura.Contexto
{
    public class SqlContextoFactory(IConfiguration configuration) : IDesignTimeDbContextFactory<Contexto>
    {
        public Contexto CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Contexto>();
            string? connection = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
            if (string.IsNullOrEmpty(connection))
                throw new InvalidOperationException("Connection:conexão não está configurada.");
            optionsBuilder.UseNpgsql(connection);
            return new Contexto(optionsBuilder.Options);
        }

    }
}
