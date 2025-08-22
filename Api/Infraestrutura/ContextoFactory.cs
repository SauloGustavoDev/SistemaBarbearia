using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Api.Infraestrutura.Contexto
{
    public class SqlContextoFactory : IDesignTimeDbContextFactory<Contexto>
    {
        public Contexto CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                       // Define o caminho base para o diretório do projeto
                       .SetBasePath(Directory.GetCurrentDirectory())
                       // Adiciona o arquivo de configuração principal
                       .AddJsonFile("appsettings.json")
                       // Adiciona o arquivo de desenvolvimento (que pode sobrescrever o principal)
                       .AddJsonFile("appsettings.Development.json", optional: true)
                       .Build();

            var optionsBuilder = new DbContextOptionsBuilder<Contexto>();


            // 🔹 Use PostgreSQL em vez de SQLite
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

            return new Contexto(optionsBuilder.Options);
        }

    }
}
