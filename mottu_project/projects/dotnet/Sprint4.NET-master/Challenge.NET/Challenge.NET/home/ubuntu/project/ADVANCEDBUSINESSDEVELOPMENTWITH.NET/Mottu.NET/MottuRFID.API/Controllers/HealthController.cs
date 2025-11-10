using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuRFID.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace MottuRFID.API.Controllers
{
    /// <summary>
    /// Controller para Health Checks da API
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HealthController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Verifica o status de saúde da API
        /// </summary>
        /// <returns>Status de saúde da aplicação</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(503)]
        public async Task<ActionResult<HealthCheckResponse>> GetHealth()
        {
            var response = new HealthCheckResponse
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
            };

            try
            {
                // Verificar conectividade com o banco de dados
                await _context.Database.CanConnectAsync();
                response.Database = new DatabaseHealth
                {
                    Status = "Healthy",
                    ResponseTime = await MeasureDatabaseResponseTime()
                };
            }
            catch (Exception ex)
            {
                response.Status = "Unhealthy";
                response.Database = new DatabaseHealth
                {
                    Status = "Unhealthy",
                    Error = ex.Message
                };
                return StatusCode(503, response);
            }

            // Verificar uso de memória
            var memoryUsage = GC.GetTotalMemory(false);
            response.Memory = new MemoryHealth
            {
                UsedBytes = memoryUsage,
                UsedMB = Math.Round(memoryUsage / 1024.0 / 1024.0, 2)
            };

            return Ok(response);
        }

        /// <summary>
        /// Verifica apenas se a API está respondendo
        /// </summary>
        /// <returns>Status básico da API</returns>
        [HttpGet("ping")]
        [ProducesResponseType(200)]
        public ActionResult<object> Ping()
        {
            return Ok(new
            {
                Status = "OK",
                Timestamp = DateTime.UtcNow,
                Message = "Mottu RFID API is running"
            });
        }

        private async Task<double> MeasureDatabaseResponseTime()
        {
            var startTime = DateTime.UtcNow;
            await _context.Database.ExecuteSqlRawAsync("SELECT 1 FROM DUAL");
            var endTime = DateTime.UtcNow;
            return (endTime - startTime).TotalMilliseconds;
        }
    }

    /// <summary>
    /// Resposta do Health Check
    /// </summary>
    public class HealthCheckResponse
    {
        /// <summary>
        /// Status geral da aplicação
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Timestamp da verificação
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Versão da aplicação
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Ambiente de execução
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// Status do banco de dados
        /// </summary>
        public DatabaseHealth Database { get; set; }

        /// <summary>
        /// Informações de memória
        /// </summary>
        public MemoryHealth Memory { get; set; }
    }

    /// <summary>
    /// Status de saúde do banco de dados
    /// </summary>
    public class DatabaseHealth
    {
        /// <summary>
        /// Status da conexão com o banco
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Tempo de resposta em milissegundos
        /// </summary>
        public double? ResponseTime { get; set; }

        /// <summary>
        /// Mensagem de erro, se houver
        /// </summary>
        public string Error { get; set; }
    }

    /// <summary>
    /// Informações de uso de memória
    /// </summary>
    public class MemoryHealth
    {
        /// <summary>
        /// Memória utilizada em bytes
        /// </summary>
        public long UsedBytes { get; set; }

        /// <summary>
        /// Memória utilizada em MB
        /// </summary>
        public double UsedMB { get; set; }
    }
}

