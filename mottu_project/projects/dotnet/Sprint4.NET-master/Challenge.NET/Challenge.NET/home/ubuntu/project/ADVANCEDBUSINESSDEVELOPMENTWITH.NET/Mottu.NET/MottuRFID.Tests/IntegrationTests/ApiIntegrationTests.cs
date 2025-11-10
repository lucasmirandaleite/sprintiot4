using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MottuRFID.Infrastructure.Data;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using MottuRFID.API;

namespace MottuRFID.Tests.IntegrationTests
{
    /// <summary>
    /// Testes de integração para a API
    /// </summary>
    public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remover o contexto de produção
                    var descriptor = services.BuildServiceProvider()
                        .GetService<DbContextOptions<ApplicationDbContext>>();
                    if (descriptor != null)
                    {
                        services.Remove(new ServiceDescriptor(typeof(DbContextOptions<ApplicationDbContext>), descriptor));
                    }

                    // Adicionar contexto em memória para testes
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDatabase");
                    });

                    // Garantir que o banco seja criado
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    context.Database.EnsureCreated();
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task HealthCheck_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/health");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Healthy", content);
        }

        [Fact]
        public async Task HealthCheck_Ping_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/health/ping");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Mottu RFID API is running", content);
        }

        [Fact]
        public async Task GetMotos_WithoutApiKey_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/motos");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetMotos_WithValidApiKey_ReturnsOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add("X-API-Key", "mottu-rfid-api-key-2024");

            // Act
            var response = await _client.GetAsync("/api/motos");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostMoto_WithValidData_ReturnsCreated()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add("X-API-Key", "mottu-rfid-api-key-2024");

            // Primeiro, criar uma filial
            var filialJson = JsonSerializer.Serialize(new
            {
                Nome = "Filial Teste",
                Endereco = "Rua Teste, 123",
                Cidade = "São Paulo",
                Estado = "SP",
                Pais = "Brasil",
                CodigoFilial = "FIL001"
            });

            var filialContent = new StringContent(filialJson, Encoding.UTF8, "application/json");
            var filialResponse = await _client.PostAsync("/api/filiais", filialContent);
            Assert.Equal(HttpStatusCode.Created, filialResponse.StatusCode);

            // Agora criar a moto
            var motoJson = JsonSerializer.Serialize(new
            {
                Placa = "ABC-1234",
                Modelo = "Honda CG 160",
                Cor = "Vermelha",
                NumeroSerie = "123456789",
                TagRFID = "RF001",
                FilialId = 1,
                Ativa = true
            });

            var motoContent = new StringContent(motoJson, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/motos", motoContent);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task SwaggerUI_IsAccessible()
        {
            // Act
            var response = await _client.GetAsync("/");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("swagger", content.ToLower());
        }

        [Fact]
        public async Task SwaggerJson_IsAccessible()
        {
            // Act
            var response = await _client.GetAsync("/swagger/v1/swagger.json");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Mottu RFID API", content);
        }

        [Fact]
        public async Task MLController_PredictNextLocation_WithoutData_ReturnsBadRequest()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add("X-API-Key", "mottu-rfid-api-key-2024");

            // Act
            var response = await _client.GetAsync("/api/ml/PredictNextLocation/1");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task MLController_AnalyzeMovementPatterns_WithoutData_ReturnsNotFound()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add("X-API-Key", "mottu-rfid-api-key-2024");

            // Act
            var response = await _client.GetAsync("/api/ml/AnalyzeMovementPatterns/1");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}

