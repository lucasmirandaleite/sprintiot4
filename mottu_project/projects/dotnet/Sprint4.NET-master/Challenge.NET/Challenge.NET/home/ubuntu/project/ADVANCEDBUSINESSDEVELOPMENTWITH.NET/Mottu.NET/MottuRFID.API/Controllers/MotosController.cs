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
    public class MotosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MotosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Motos
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<MotoDTO>>> GetMotos(
            [FromQuery] string? placa = null,
            [FromQuery] string? modelo = null,
            [FromQuery] int? filialId = null,
            [FromQuery] bool? ativa = null)
        {
            var query = _context.Motos
                .Include(m => m.Filial)
                .Include(m => m.PontoLeituraAtual)
                .AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(placa))
                query = query.Where(m => m.Placa.Contains(placa));

            if (!string.IsNullOrEmpty(modelo))
                query = query.Where(m => m.Modelo.Contains(modelo));

            if (filialId.HasValue)
                query = query.Where(m => m.FilialId == filialId.Value);

            if (ativa.HasValue)
                query = query.Where(m => m.Ativa == ativa.Value);

            var motos = await query.ToListAsync();

            return Ok(motos.Select(m => new MotoDTO
            {
                Id = m.Id,
                Placa = m.Placa,
                Modelo = m.Modelo,
                Cor = m.Cor,
                NumeroSerie = m.NumeroSerie,
                TagRFID = m.TagRFID,
                FilialId = m.FilialId,
                FilialNome = m.Filial?.Nome,
                PontoLeituraAtualId = m.PontoLeituraAtualId,
                PontoLeituraAtualNome = m.PontoLeituraAtual?.Nome,
                UltimaAtualizacao = m.UltimaAtualizacao,
                Ativa = m.Ativa
            }));
        }

        // GET: api/Motos/5
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MotoDTO>> GetMoto(int id)
        {
            var moto = await _context.Motos
                .Include(m => m.Filial)
                .Include(m => m.PontoLeituraAtual)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (moto == null)
            {
                return NotFound();
            }

            return Ok(new MotoDTO
            {
                Id = moto.Id,
                Placa = moto.Placa,
                Modelo = moto.Modelo,
                Cor = moto.Cor,
                NumeroSerie = moto.NumeroSerie,
                TagRFID = moto.TagRFID,
                FilialId = moto.FilialId,
                FilialNome = moto.Filial?.Nome,
                PontoLeituraAtualId = moto.PontoLeituraAtualId,
                PontoLeituraAtualNome = moto.PontoLeituraAtual?.Nome,
                UltimaAtualizacao = moto.UltimaAtualizacao,
                Ativa = moto.Ativa
            });
        }

        // GET: api/Motos/Tag/RF123456
        [HttpGet("Tag/{tagRFID}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MotoDTO>> GetMotoByTag(string tagRFID)
        {
            var moto = await _context.Motos
                .Include(m => m.Filial)
                .Include(m => m.PontoLeituraAtual)
                .FirstOrDefaultAsync(m => m.TagRFID == tagRFID);

            if (moto == null)
            {
                return NotFound();
            }

            return Ok(new MotoDTO
            {
                Id = moto.Id,
                Placa = moto.Placa,
                Modelo = moto.Modelo,
                Cor = moto.Cor,
                NumeroSerie = moto.NumeroSerie,
                TagRFID = moto.TagRFID,
                FilialId = moto.FilialId,
                FilialNome = moto.Filial?.Nome,
                PontoLeituraAtualId = moto.PontoLeituraAtualId,
                PontoLeituraAtualNome = moto.PontoLeituraAtual?.Nome,
                UltimaAtualizacao = moto.UltimaAtualizacao,
                Ativa = moto.Ativa
            });
        }

        // GET: api/Motos/Filial/5
        [HttpGet("Filial/{filialId}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<MotoDTO>>> GetMotosByFilial(int filialId)
        {
            var motos = await _context.Motos
                .Include(m => m.Filial)
                .Include(m => m.PontoLeituraAtual)
                .Where(m => m.FilialId == filialId)
                .ToListAsync();

            return Ok(motos.Select(m => new MotoDTO
            {
                Id = m.Id,
                Placa = m.Placa,
                Modelo = m.Modelo,
                Cor = m.Cor,
                NumeroSerie = m.NumeroSerie,
                TagRFID = m.TagRFID,
                FilialId = m.FilialId,
                FilialNome = m.Filial?.Nome,
                PontoLeituraAtualId = m.PontoLeituraAtualId,
                PontoLeituraAtualNome = m.PontoLeituraAtual?.Nome,
                UltimaAtualizacao = m.UltimaAtualizacao,
                Ativa = m.Ativa
            }));
        }

        // POST: api/Motos
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<MotoDTO>> PostMoto(MotoDTO motoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar se a filial existe
            var filial = await _context.Filiais.FindAsync(motoDto.FilialId);
            if (filial == null)
            {
                return BadRequest("Filial não encontrada");
            }

            // Verificar se o ponto de leitura existe, se fornecido
            if (motoDto.PontoLeituraAtualId.HasValue)
            {
                var pontoLeitura = await _context.PontosLeitura.FindAsync(motoDto.PontoLeituraAtualId.Value);
                if (pontoLeitura == null)
                {
                    return BadRequest("Ponto de leitura não encontrado");
                }
            }

            // Verificar se já existe uma moto com a mesma placa
            var motoExistente = await _context.Motos.FirstOrDefaultAsync(m => m.Placa == motoDto.Placa);
            if (motoExistente != null)
            {
                return BadRequest("Já existe uma moto cadastrada com esta placa");
            }

            // Verificar se já existe uma moto com a mesma tag RFID
            motoExistente = await _context.Motos.FirstOrDefaultAsync(m => m.TagRFID == motoDto.TagRFID);
            if (motoExistente != null)
            {
                return BadRequest("Já existe uma moto cadastrada com esta tag RFID");
            }

            var moto = new Moto
            {
                Placa = motoDto.Placa,
                Modelo = motoDto.Modelo,
                Cor = motoDto.Cor,
                NumeroSerie = motoDto.NumeroSerie,
                TagRFID = motoDto.TagRFID,
                FilialId = motoDto.FilialId,
                PontoLeituraAtualId = motoDto.PontoLeituraAtualId,
                UltimaAtualizacao = DateTime.Now,
                Ativa = motoDto.Ativa
            };

            _context.Motos.Add(moto);
            await _context.SaveChangesAsync();

            motoDto.Id = moto.Id;
            motoDto.UltimaAtualizacao = moto.UltimaAtualizacao;

            return CreatedAtAction(nameof(GetMoto), new { id = moto.Id }, motoDto);
        }

        // PUT: api/Motos/5
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutMoto(int id, MotoDTO motoDto)
        {
            if (id != motoDto.Id)
            {
                return BadRequest("O ID da moto não corresponde ao ID da URL");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var moto = await _context.Motos.FindAsync(id);
            if (moto == null)
            {
                return NotFound();
            }

            // Verificar se a filial existe
            var filial = await _context.Filiais.FindAsync(motoDto.FilialId);
            if (filial == null)
            {
                return BadRequest("Filial não encontrada");
            }

            // Verificar se o ponto de leitura existe, se fornecido
            if (motoDto.PontoLeituraAtualId.HasValue)
            {
                var pontoLeitura = await _context.PontosLeitura.FindAsync(motoDto.PontoLeituraAtualId.Value);
                if (pontoLeitura == null)
                {
                    return BadRequest("Ponto de leitura não encontrado");
                }
            }

            // Verificar se já existe outra moto com a mesma placa
            var motoExistente = await _context.Motos.FirstOrDefaultAsync(m => m.Placa == motoDto.Placa && m.Id != id);
            if (motoExistente != null)
            {
                return BadRequest("Já existe outra moto cadastrada com esta placa");
            }

            // Verificar se já existe outra moto com a mesma tag RFID
            motoExistente = await _context.Motos.FirstOrDefaultAsync(m => m.TagRFID == motoDto.TagRFID && m.Id != id);
            if (motoExistente != null)
            {
                return BadRequest("Já existe outra moto cadastrada com esta tag RFID");
            }

            moto.Placa = motoDto.Placa;
            moto.Modelo = motoDto.Modelo;
            moto.Cor = motoDto.Cor;
            moto.NumeroSerie = motoDto.NumeroSerie;
            moto.TagRFID = motoDto.TagRFID;
            moto.FilialId = motoDto.FilialId;
            moto.PontoLeituraAtualId = motoDto.PontoLeituraAtualId;
            moto.UltimaAtualizacao = DateTime.Now;
            moto.Ativa = motoDto.Ativa;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MotoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Motos/5
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteMoto(int id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null)
            {
                return NotFound();
            }

            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MotoExists(int id)
        {
            return _context.Motos.Any(e => e.Id == id);
        }
    }
}
