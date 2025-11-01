using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Api.Infraestrutura.Contexto
{
    public class ContextoFactory : IDesignTimeDbContextFactory<Contexto>
    {
        public Contexto CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<Contexto>();
            string? connection = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
            if (string.IsNullOrEmpty(connection))
                throw new InvalidOperationException("Connection:conexão não está configurada.");
            optionsBuilder.UseNpgsql(connection);
            return new Contexto(optionsBuilder.Options);
        }
    }
}
