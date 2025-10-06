using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Sprint1.Domain.Repositories;
using Sprint1.Infrastructure.Data;
using Sprint1.Mappings;
using Sprint1.UseCase.Usuario;

namespace Sprint1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddAuthorization();

            // Entity Framework - Oracle
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseOracle(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    oracleOptions =>
                    {
                        // Configurar timeout de conexão (30 segundos)
                        oracleOptions.CommandTimeout(30);
                    });
                
                // Habilitar logging sensível apenas em desenvolvimento
                if (builder.Environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            }, ServiceLifetime.Scoped);

            // Repositories
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            // Use Cases
            builder.Services.AddScoped<CreateUsuarioUseCase>();
            builder.Services.AddScoped<GetUsuarioByIdUseCase>();
            builder.Services.AddScoped<GetAllUsuariosUseCase>();
            builder.Services.AddScoped<AlterarEmailUsuarioUseCase>();
            builder.Services.AddScoped<AlterarSenhaUsuarioUseCase>();
            builder.Services.AddScoped<DeleteUsuarioUseCase>();

            // AutoMapper
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<UsuarioMappingProfile>();
            });

            // FluentValidation
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            // Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Sprint1 API",
                    Version = "v1",
                    Description = "API para gerenciamento de usuários com Oracle Autonomous Database"
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
