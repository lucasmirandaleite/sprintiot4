using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MottuRFID.Infrastructure.Data;
using System;
using System.IO;

namespace MottuRFID.Infrastructure
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Obter o diretório do projeto
            var basePath = Directory.GetCurrentDirectory();
            Console.WriteLine($"Diretório atual: {basePath}");

            // Configurar o builder para ler o appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
                .Build();

            // Obter a string de conexão
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Configurar as opções do DbContext
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseOracle(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
