using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuRFID.Application.DTOs;
using MottuRFID.Domain.Entities;
using MottuRFID.Infrastructure.Data;
using System.ComponentModel.DataAnnotations;

namespace MottuRFID.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeiturasRFIDController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LeiturasRFIDController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/LeiturasRFID
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<LeituraRFIDDTO>>> GetLeiturasRFID(
            [FromQuery] string? tagRFID = null,
            [FromQuery] int? motoId = null,
            [FromQuery] int? pontoLeituraId = null,
            [FromQuery] DateTime? dataInicio = null,
            [FromQuery] DateTime? dataFim = null)
        {
            var query = _context.LeiturasRFID
                .Include(l => l.Moto)
                .Include(l => l.PontoLeitura)
                .AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(tagRFID))
                query = query.Where(l => l.TagRFID == tagRFID);

            if (motoId.HasValue)
                query = query.Where(l => l.MotoId == motoId.Value);

            if (pontoLeituraId.HasValue)
                query = query.Where(l => l.PontoLeituraId == pontoLeituraId.Value);

            if (dataInicio.HasValue)
                query = query.Where(l => l.DataHoraLeitura >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(l => l.DataHoraLeitura <= dataFim.Value);

            // Ordenar por data/hora decrescente
            query = query.OrderByDescending(l => l.DataHoraLeitura);

            var leituras = await query.ToListAsync();

            return Ok(leituras.Select(l => new LeituraRFIDDTO
            {
                Id = l.Id,
                TagRFID = l.TagRFID,
                MotoId = l.MotoId,
                MotoPlaca = l.Moto?.Placa,
                MotoModelo = l.Moto?.Modelo,
                PontoLeituraId = l.PontoLeituraId,
                PontoLeituraNome = l.PontoLeitura?.Nome,
                DataHoraLeitura = l.DataHoraLeitura,
                Observacao = l.Observacao
            }));
        }

        // GET: api/LeiturasRFID/5
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<LeituraRFIDDTO>> GetLeituraRFID(int id)
        {
            var leituraRFID = await _context.LeiturasRFID
                .Include(l => l.Moto)
                .Include(l => l.PontoLeitura)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leituraRFID == null)
            {
                return NotFound();
            }

            return Ok(new LeituraRFIDDTO
            {
                Id = leituraRFID.Id,
                TagRFID = leituraRFID.TagRFID,
                MotoId = leituraRFID.MotoId,
                MotoPlaca = leituraRFID.Moto?.Placa,
                MotoModelo = leituraRFID.Moto?.Modelo,
                PontoLeituraId = leituraRFID.PontoLeituraId,
                PontoLeituraNome = leituraRFID.PontoLeitura?.Nome,
                DataHoraLeitura = leituraRFID.DataHoraLeitura,
                Observacao = leituraRFID.Observacao
            });
        }

        // GET: api/LeiturasRFID/Moto/5
        [HttpGet("Moto/{motoId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<LeituraRFIDDTO>>> GetLeiturasByMoto(int motoId)
        {
            var moto = await _context.Motos.FindAsync(motoId);
            if (moto == null)
            {
                return NotFound("Moto não encontrada");
            }

            var leituras = await _context.LeiturasRFID
                .Include(l => l.PontoLeitura)
                .Where(l => l.MotoId == motoId)
                .OrderByDescending(l => l.DataHoraLeitura)
                .ToListAsync();

            return Ok(leituras.Select(l => new LeituraRFIDDTO
            {
                Id = l.Id,
                TagRFID = l.TagRFID,
                MotoId = l.MotoId,
                MotoPlaca = moto.Placa,
                MotoModelo = moto.Modelo,
                PontoLeituraId = l.PontoLeituraId,
                PontoLeituraNome = l.PontoLeitura?.Nome,
                DataHoraLeitura = l.DataHoraLeitura,
                Observacao = l.Observacao
            }));
        }

        // GET: api/LeiturasRFID/PontoLeitura/5
        [HttpGet("PontoLeitura/{pontoId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<LeituraRFIDDTO>>> GetLeiturasByPontoLeitura(int pontoId)
        {
            var pontoLeitura = await _context.PontosLeitura.FindAsync(pontoId);
            if (pontoLeitura == null)
            {
                return NotFound("Ponto de leitura não encontrado");
            }

            var leituras = await _context.LeiturasRFID
                .Include(l => l.Moto)
                .Where(l => l.PontoLeituraId == pontoId)
                .OrderByDescending(l => l.DataHoraLeitura)
                .ToListAsync();

            return Ok(leituras.Select(l => new LeituraRFIDDTO
            {
                Id = l.Id,
                TagRFID = l.TagRFID,
                MotoId = l.MotoId,
                MotoPlaca = l.Moto?.Placa,
                MotoModelo = l.Moto?.Modelo,
                PontoLeituraId = l.PontoLeituraId,
                PontoLeituraNome = pontoLeitura.Nome,
                DataHoraLeitura = l.DataHoraLeitura,
                Observacao = l.Observacao
            }));
        }

        // POST: api/LeiturasRFID
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<LeituraRFIDDTO>> PostLeituraRFID(LeituraRFIDDTO leituraDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar se a tag RFID existe e obter a moto correspondente
            var moto = await _context.Motos.FirstOrDefaultAsync(m => m.TagRFID == leituraDto.TagRFID);
            if (moto == null)
            {
                return BadRequest("Tag RFID não encontrada em nenhuma moto cadastrada");
            }

            // Verificar se o ponto de leitura existe
            var pontoLeitura = await _context.PontosLeitura.FindAsync(leituraDto.PontoLeituraId);
            if (pontoLeitura == null)
            {
                return BadRequest("Ponto de leitura não encontrado");
            }

            // Criar a leitura RFID
            var leituraRFID = new LeituraRFID
            {
                TagRFID = leituraDto.TagRFID,
                MotoId = moto.Id,
                PontoLeituraId = leituraDto.PontoLeituraId,
                DataHoraLeitura = leituraDto.DataHoraLeitura,
                Observacao = leituraDto.Observacao
            };

            // Atualizar a posição atual da moto
            moto.PontoLeituraAtualId = leituraDto.PontoLeituraId;
            moto.UltimaAtualizacao = DateTime.Now;

            _context.LeiturasRFID.Add(leituraRFID);
            await _context.SaveChangesAsync();

            leituraDto.Id = leituraRFID.Id;
            leituraDto.MotoId = moto.Id;
            leituraDto.MotoPlaca = moto.Placa;
            leituraDto.MotoModelo = moto.Modelo;
            leituraDto.PontoLeituraNome = pontoLeitura.Nome;

            return CreatedAtAction(nameof(GetLeituraRFID), new { id = leituraRFID.Id }, leituraDto);
        }

        // DELETE: api/LeiturasRFID/5
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteLeituraRFID(int id)
        {
            var leituraRFID = await _context.LeiturasRFID.FindAsync(id);
            if (leituraRFID == null)
            {
                return NotFound();
            }

            _context.LeiturasRFID.Remove(leituraRFID);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
