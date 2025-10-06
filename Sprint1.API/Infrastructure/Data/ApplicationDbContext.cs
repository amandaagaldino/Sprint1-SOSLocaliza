using Microsoft.EntityFrameworkCore;
using Sprint1.Domain.Entities;

namespace Sprint1.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        
        // Configurações adicionais para Oracle
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseOracle(
                "Data Source=(DESCRIPTION=(RETRY_COUNT=20)(RETRY_DELAY=3)(ADDRESS=(PROTOCOL=TCPS)(PORT=1522)(HOST=adb.sa-saopaulo-1.oraclecloud.com))(CONNECT_DATA=(SERVICE_NAME=g8befe05b93110f_soslocaliza_high.adb.oraclecloud.com))(SECURITY=(SSL_SERVER_DN_MATCH=yes)));User Id=admin;Password=SOSLocaliza;Wallet_Location=/Users/amandagaldino/Documents/Wallet_SOSLocaliza",
                oracleOptions =>
                {
                    // Timeout de 30 segundos para comandos
                    oracleOptions.CommandTimeout(30);
                });
        }
    }

    public override void Dispose()
    {
        try
        {
            // Fechar conexão antes de descartar
            var connection = Database.GetDbConnection();
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }
        catch
        {
            // Ignorar erros ao fechar
        }
        finally
        {
            base.Dispose();
        }
    }

    public override async ValueTask DisposeAsync()
    {
        try
        {
            // Fechar conexão antes de descartar (assíncrono)
            var connection = Database.GetDbConnection();
            if (connection.State == System.Data.ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }
        catch
        {
            // Ignorar erros ao fechar
        }
        finally
        {
            await base.DisposeAsync();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            // Configuração para Oracle
            entity.ToTable("USUARIOS");
            
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.NomeCompleto).IsRequired().HasMaxLength(100).HasColumnName("NOME_COMPLETO");
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100).HasColumnName("EMAIL");
            entity.Property(e => e.Senha).IsRequired().HasMaxLength(255).HasColumnName("SENHA");
            entity.Property(e => e.Cpf).IsRequired().HasMaxLength(11).HasColumnName("CPF");
            entity.Property(e => e.DataNascimento).IsRequired().HasColumnName("DATA_NASCIMENTO");
            entity.Property(e => e.DataCriacao).IsRequired().HasColumnName("DATA_CRIACAO");
            entity.Property(e => e.DataAtualizacao).HasColumnName("DATA_ATUALIZACAO");
            entity.Property(e => e.Ativo).IsRequired().HasColumnName("ATIVO");

            // Índices únicos
            entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("IX_USUARIOS_EMAIL");
            entity.HasIndex(e => e.Cpf).IsUnique().HasDatabaseName("IX_USUARIOS_CPF");
        });
    }
}
