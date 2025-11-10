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
    public class FiliaisController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FiliaisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Filiais
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<FilialDTO>>> GetFiliais()
        {
            var filiais = await _context.Filiais
                .Include(f => f.Motos)
                .Include(f => f.PontosLeitura)
                .ToListAsync();

            return Ok(filiais.Select(f => new FilialDTO
            {
                Id = f.Id,
                Nome = f.Nome,
                Endereco = f.Endereco,
                Cidade = f.Cidade,
                Estado = f.Estado,
                Pais = f.Pais,
                CodigoFilial = f.CodigoFilial,
                QuantidadeMotos = f.Motos?.Count ?? 0,
                QuantidadePontosLeitura = f.PontosLeitura?.Count ?? 0
            }));
        }

        // GET: api/Filiais/5
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<FilialDTO>> GetFilial(int id)
        {
            var filial = await _context.Filiais
                .Include(f => f.Motos)
                .Include(f => f.PontosLeitura)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (filial == null)
            {
                return NotFound();
            }

            return Ok(new FilialDTO
            {
                Id = filial.Id,
                Nome = filial.Nome,
                Endereco = filial.Endereco,
                Cidade = filial.Cidade,
                Estado = filial.Estado,
                Pais = filial.Pais,
                CodigoFilial = filial.CodigoFilial,
                QuantidadeMotos = filial.Motos?.Count ?? 0,
                QuantidadePontosLeitura = filial.PontosLeitura?.Count ?? 0
            });
        }

        // GET: api/Filiais/5/Motos
        [HttpGet("{id}/Motos")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<MotoDTO>>> GetMotosFilial(int id)
        {
            var filial = await _context.Filiais.FindAsync(id);
            if (filial == null)
            {
                return NotFound();
            }

            var motos = await _context.Motos
                .Include(m => m.PontoLeituraAtual)
                .Where(m => m.FilialId == id)
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
                FilialNome = filial.Nome,
                PontoLeituraAtualId = m.PontoLeituraAtualId,
                PontoLeituraAtualNome = m.PontoLeituraAtual?.Nome,
                UltimaAtualizacao = m.UltimaAtualizacao,
                Ativa = m.Ativa
            }));
        }

        // GET: api/Filiais/5/PontosLeitura
        [HttpGet("{id}/PontosLeitura")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<PontoLeituraDTO>>> GetPontosLeituraFilial(int id)
        {
            var filial = await _context.Filiais.FindAsync(id);
            if (filial == null)
            {
                return NotFound();
            }

            var pontosLeitura = await _context.PontosLeitura
                .Where(p => p.FilialId == id)
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

        // POST: api/Filiais
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<FilialDTO>> PostFilial(FilialDTO filialDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar se já existe uma filial com o mesmo código
            var filialExistente = await _context.Filiais.FirstOrDefaultAsync(f => f.CodigoFilial == filialDto.CodigoFilial);
            if (filialExistente != null)
            {
                return BadRequest("Já existe uma filial cadastrada com este código");
            }

            var filial = new Filial
            {
                Nome = filialDto.Nome,
                Endereco = filialDto.Endereco,
                Cidade = filialDto.Cidade,
                Estado = filialDto.Estado,
                Pais = filialDto.Pais,
                CodigoFilial = filialDto.CodigoFilial
            };

            _context.Filiais.Add(filial);
            await _context.SaveChangesAsync();

            filialDto.Id = filial.Id;

            return CreatedAtAction(nameof(GetFilial), new { id = filial.Id }, filialDto);
        }

        // PUT: api/Filiais/5
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutFilial(int id, FilialDTO filialDto)
        {
            if (id != filialDto.Id)
            {
                return BadRequest("O ID da filial não corresponde ao ID da URL");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var filial = await _context.Filiais.FindAsync(id);
            if (filial == null)
            {
                return NotFound();
            }

            // Verificar se já existe outra filial com o mesmo código
            var filialExistente = await _context.Filiais.FirstOrDefaultAsync(f => f.CodigoFilial == filialDto.CodigoFilial && f.Id != id);
            if (filialExistente != null)
            {
                return BadRequest("Já existe outra filial cadastrada com este código");
            }

            filial.Nome = filialDto.Nome;
            filial.Endereco = filialDto.Endereco;
            filial.Cidade = filialDto.Cidade;
            filial.Estado = filialDto.Estado;
            filial.Pais = filialDto.Pais;
            filial.CodigoFilial = filialDto.CodigoFilial;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FilialExists(id))
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

        // DELETE: api/Filiais/5
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteFilial(int id)
        {
            var filial = await _context.Filiais
                .Include(f => f.Motos)
                .Include(f => f.PontosLeitura)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (filial == null)
            {
                return NotFound();
            }

            // Verificar se existem motos ou pontos de leitura associados
            if ((filial.Motos != null && filial.Motos.Any()) || 
                (filial.PontosLeitura != null && filial.PontosLeitura.Any()))
            {
                return BadRequest("Não é possível excluir a filial pois existem motos ou pontos de leitura associados");
            }

            _context.Filiais.Remove(filial);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FilialExists(int id)
        {
            return _context.Filiais.Any(e => e.Id == id);
        }
    }
}
