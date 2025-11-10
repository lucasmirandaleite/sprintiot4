using Microsoft.AspNetCore.Mvc;
using MottuRFID.Application.DTOs;
using MottuRFID.Application.Services;
using System.Collections.Generic;

namespace MottuRFID.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly MotoStatusService _motoStatusService;

        public DashboardController(MotoStatusService motoStatusService)
        {
            _motoStatusService = motoStatusService;
        }

        /// <summary>
        /// Retorna o status e localização de todas as motos para o Dashboard.
        /// </summary>
        [HttpGet("status")]
        [ProducesResponseType(typeof(IEnumerable<MotoStatusDTO>), StatusCodes.Status200OK)]
        public IActionResult GetMotoStatuses()
        {
            var statuses = _motoStatusService.GetAllStatuses();
            return Ok(statuses);
        }
    }
}
