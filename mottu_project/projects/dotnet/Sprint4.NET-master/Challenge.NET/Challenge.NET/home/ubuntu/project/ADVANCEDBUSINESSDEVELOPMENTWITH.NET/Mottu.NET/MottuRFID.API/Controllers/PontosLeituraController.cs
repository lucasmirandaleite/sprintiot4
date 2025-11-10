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
    public class PontosLeituraController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PontosLeituraController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PontosLeitura
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<PontoLeituraDTO>>> GetPontosLeitura(
            [FromQuery] int? filialId = null,
            [FromQuery] bool? ativo = null)
        {
            var query = _context.PontosLeitura
                .Include(p => p.Filial)
                .AsQueryable();

            // Aplicar filtros
            if (filialId.HasValue)
                query = query.Where(p => p.FilialId == filialId.Value);

            if (ativo.HasValue)
                query = query.Where(p => p.Ativo == ativo.Value);

            var pontosLeitura = await query.ToListAsync();

            return Ok(pontosLeitura.Select(p => new PontoLeituraDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Descricao = p.Descricao,
                Localizacao = p.Localizacao,
                FilialId = p.FilialId,
                FilialNome = p.Filial?.Nome,
                PosicaoX = p.PosicaoX,
                PosicaoY = p.PosicaoY,
                Ativo = p.Ativo,
                QuantidadeMotosAtuais = _context.Motos.Count(m => m.PontoLeituraAtualId == p.Id)
            }));
        }

        // GET: api/PontosLeitura/5
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<PontoLeituraDTO>> GetPontoLeitura(int id)
        {
            var pontoLeitura = await _context.PontosLeitura
                .Include(p => p.Filial)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pontoLeitura == null)
            {
                return NotFound();
            }

            return Ok(new PontoLeituraDTO
            {
                Id = pontoLeitura.Id,
                Nome = pontoLeitura.Nome,
                Descricao = pontoLeitura.Descricao,
                Localizacao = pontoLeitura.Localizacao,
                FilialId = pontoLeitura.FilialId,
                FilialNome = pontoLeitura.Filial?.Nome,
                PosicaoX = pontoLeitura.PosicaoX,
                PosicaoY = pontoLeitura.PosicaoY,
                Ativo = pontoLeitura.Ativo,
                QuantidadeMotosAtuais = _context.Motos.Count(m => m.PontoLeituraAtualId == pontoLeitura.Id)
            });
        }

        // GET: api/PontosLeitura/Filial/5
        [HttpGet("Filial/{filialId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<PontoLeituraDTO>>> GetPontosLeituraByFilial(int filialId)
        {
            var filial = await _context.Filiais.FindAsync(filialId);
            if (filial == null)
            {
                return NotFound("Filial não encontrada");
            }

            var pontosLeitura = await _context.PontosLeitura
                .Where(p => p.FilialId == filialId)
                .ToListAsync();

            return Ok(pontosLeitura.Select(p => new PontoLeituraDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Descricao = p.Descricao,
                Localizacao = p.Localizacao,
                FilialId = p.FilialId,
                FilialNome = filial.Nome,
                PosicaoX = p.PosicaoX,
                PosicaoY = p.PosicaoY,
                Ativo = p.Ativo,
                QuantidadeMotosAtuais = _context.Motos.Count(m => m.PontoLeituraAtualId == p.Id)
            }));
        }

        // GET: api/PontosLeitura/5/Motos
        [HttpGet("{id}/Motos")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<MotoDTO>>> GetMotosByPontoLeitura(int id)
        {
            var pontoLeitura = await _context.PontosLeitura.FindAsync(id);
            if (pontoLeitura == null)
            {
                return NotFound("Ponto de leitura não encontrado");
            }

            var motos = await _context.Motos
                .Include(m => m.Filial)
                .Where(m => m.PontoLeituraAtualId == id)
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
                PontoLeituraAtualNome = pontoLeitura.Nome,
                UltimaAtualizacao = m.UltimaAtualizacao,
                Ativa = m.Ativa
            }));
        }

        // POST: api/PontosLeitura
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<PontoLeituraDTO>> PostPontoLeitura(PontoLeituraDTO pontoLeituraDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar se a filial existe
            var filial = await _context.Filiais.FindAsync(pontoLeituraDto.FilialId);
            if (filial == null)
            {
                return BadRequest("Filial não encontrada");
            }

            var pontoLeitura = new PontoLeitura
            {
                Nome = pontoLeituraDto.Nome,
                Descricao = pontoLeituraDto.Descricao,
                Localizacao = pontoLeituraDto.Localizacao,
                FilialId = pontoLeituraDto.FilialId,
                PosicaoX = pontoLeituraDto.PosicaoX,
                PosicaoY = pontoLeituraDto.PosicaoY,
                Ativo = pontoLeituraDto.Ativo
            };

            _context.PontosLeitura.Add(pontoLeitura);
            await _context.SaveChangesAsync();

            pontoLeituraDto.Id = pontoLeitura.Id;
            pontoLeituraDto.FilialNome = filial.Nome;

            return CreatedAtAction(nameof(GetPontoLeitura), new { id = pontoLeitura.Id }, pontoLeituraDto);
        }

        // PUT: api/PontosLeitura/5
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutPontoLeitura(int id, PontoLeituraDTO pontoLeituraDto)
        {
            if (id != pontoLeituraDto.Id)
            {
                return BadRequest("O ID do ponto de leitura não corresponde ao ID da URL");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pontoLeitura = await _context.PontosLeitura.FindAsync(id);
            if (pontoLeitura == null)
            {
                return NotFound();
            }

            // Verificar se a filial existe
            var filial = await _context.Filiais.FindAsync(pontoLeituraDto.FilialId);
            if (filial == null)
            {
                return BadRequest("Filial não encontrada");
            }

            pontoLeitura.Nome = pontoLeituraDto.Nome;
            pontoLeitura.Descricao = pontoLeituraDto.Descricao;
            pontoLeitura.Localizacao = pontoLeituraDto.Localizacao;
            pontoLeitura.FilialId = pontoLeituraDto.FilialId;
            pontoLeitura.PosicaoX = pontoLeituraDto.PosicaoX;
            pontoLeitura.PosicaoY = pontoLeituraDto.PosicaoY;
            pontoLeitura.Ativo = pontoLeituraDto.Ativo;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PontoLeituraExists(id))
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

        // DELETE: api/PontosLeitura/5
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeletePontoLeitura(int id)
        {
            var pontoLeitura = await _context.PontosLeitura.FindAsync(id);
            if (pontoLeitura == null)
            {
                return NotFound();
            }

            // Verificar se existem motos associadas a este ponto de leitura
            var motosAssociadas = await _context.Motos.AnyAsync(m => m.PontoLeituraAtualId == id);
            if (motosAssociadas)
            {
                return BadRequest("Não é possível excluir o ponto de leitura pois existem motos associadas");
            }

            // Verificar se existem leituras RFID associadas a este ponto de leitura
            var leiturasAssociadas = await _context.LeiturasRFID.AnyAsync(l => l.PontoLeituraId == id);
            if (leiturasAssociadas)
            {
                return BadRequest("Não é possível excluir o ponto de leitura pois existem leituras RFID associadas");
            }

            _context.PontosLeitura.Remove(pontoLeitura);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PontoLeituraExists(int id)
        {
            return _context.PontosLeitura.Any(e => e.Id == id);
        }
    }
}
