using Microsoft.AspNetCore.Mvc;
using MottuRFID.Application.DTOs;
using MottuRFID.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MottuRFID.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class VisionController : ControllerBase
    {
        private readonly ILogger<VisionController> _logger;
        private readonly MotoStatusService _motoStatusService;

        public VisionController(ILogger<VisionController> logger, MotoStatusService motoStatusService)
        {
            _logger = logger;
            _motoStatusService = motoStatusService;
        }

        /// <summary>
        /// Recebe dados de detecção da Visão Computacional (Python).
        /// </summary>
        [HttpPost("detection")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostDetection([FromBody] VisionDetectionDTO detection)
        {
            if (detection == null)
            {
                return BadRequest("Dados de detecção inválidos.");
            }

            // AQUI VOCÊ ADICIONARIA A LÓGICA DE NEGÓCIO:
            // 1. Buscar a moto no banco de dados (usando a tag ou ID, se o YOLO pudesse identificar).
            // 2. Atualizar o status/localização da moto com base na detecção.
            // 3. Persistir o registro da detecção.
            _motoStatusService.UpdateStatusFromVision(detection);

            _logger.LogInformation(
                "Recebida detecção de Visão Computacional: {Count} motos em {Source} no frame {FrameId} em {Timestamp}",
                detection.MotoCount, detection.Source, detection.FrameId, detection.Timestamp
            );

            // Retorna 200 OK para o script Python continuar.
            return Ok(new { message = "Detecção recebida com sucesso." });
        }

        /// <summary>
        /// Retorna o status atual de todas as motos para o Dashboard.
        /// </summary>
        [HttpGet("latest")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MotoStatusDTO>))]
        public IActionResult GetLatestStatus()
        {
            var statuses = _motoStatusService.GetAllStatuses();
            return Ok(statuses);
        }
    }
}
