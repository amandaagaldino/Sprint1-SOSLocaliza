using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sprint1.Infrastructure.Data;

namespace Sprint1.Migrations
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Carregar configuração do appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Obter connection string
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Configurar DbContext com Oracle
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseOracle(connectionString, oracleOptions =>
            {
                oracleOptions.CommandTimeout(30);
            });

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
