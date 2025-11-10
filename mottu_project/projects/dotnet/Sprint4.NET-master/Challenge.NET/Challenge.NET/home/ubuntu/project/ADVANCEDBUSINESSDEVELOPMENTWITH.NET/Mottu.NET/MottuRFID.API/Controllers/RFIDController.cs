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
    public class RFIDController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RFIDController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Registra uma nova leitura RFID e atualiza a posição da moto
        /// </summary>
        /// <param name="leituraRequest">Dados da leitura RFID</param>
        /// <returns>Resultado da operação com detalhes da moto e ponto de leitura</returns>
        [HttpPost("Registrar")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<RegistroLeituraResponse>> RegistrarLeitura(RegistroLeituraRequest leituraRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar se a tag RFID existe e obter a moto correspondente
            var moto = await _context.Motos
                .Include(m => m.Filial)
                .FirstOrDefaultAsync(m => m.TagRFID == leituraRequest.TagRFID);
            
            if (moto == null)
            {
                return NotFound($"Tag RFID '{leituraRequest.TagRFID}' não encontrada em nenhuma moto cadastrada");
            }

            // Verificar se o ponto de leitura existe
            var pontoLeitura = await _context.PontosLeitura
                .Include(p => p.Filial)
                .FirstOrDefaultAsync(p => p.Id == leituraRequest.PontoLeituraId);
            
            if (pontoLeitura == null)
            {
                return NotFound($"Ponto de leitura ID {leituraRequest.PontoLeituraId} não encontrado");
            }

            // Verificar se a moto e o ponto de leitura pertencem à mesma filial
            if (moto.FilialId != pontoLeitura.FilialId)
            {
                return BadRequest($"A moto pertence à filial '{moto.Filial?.Nome}' mas o ponto de leitura pertence à filial '{pontoLeitura.Filial?.Nome}'");
            }

            // Criar a leitura RFID
            var leituraRFID = new LeituraRFID
            {
                TagRFID = leituraRequest.TagRFID,
                MotoId = moto.Id,
                PontoLeituraId = leituraRequest.PontoLeituraId,
                DataHoraLeitura = DateTime.Now,
                Observacao = leituraRequest.Observacao
            };

            // Atualizar a posição atual da moto
            moto.PontoLeituraAtualId = leituraRequest.PontoLeituraId;
            moto.UltimaAtualizacao = DateTime.Now;

            _context.LeiturasRFID.Add(leituraRFID);
            await _context.SaveChangesAsync();

            // Preparar resposta
            var response = new RegistroLeituraResponse
            {
                LeituraId = leituraRFID.Id,
                DataHoraLeitura = leituraRFID.DataHoraLeitura,
                Moto = new MotoInfo
                {
                    Id = moto.Id,
                    Placa = moto.Placa,
                    Modelo = moto.Modelo,
                    Cor = moto.Cor,
                    TagRFID = moto.TagRFID
                },
                PontoLeitura = new PontoLeituraInfo
                {
                    Id = pontoLeitura.Id,
                    Nome = pontoLeitura.Nome,
                    Localizacao = pontoLeitura.Localizacao,
                    PosicaoX = pontoLeitura.PosicaoX,
                    PosicaoY = pontoLeitura.PosicaoY
                },
                Filial = new FilialInfo
                {
                    Id = moto.FilialId,
                    Nome = moto.Filial?.Nome,
                    CodigoFilial = moto.Filial?.CodigoFilial
                }
            };

            return Ok(response);
        }

        /// <summary>
        /// Obtém o histórico de leituras RFID de uma moto específica
        /// </summary>
        /// <param name="motoId">ID da moto</param>
        /// <param name="dataInicio">Data de início opcional para filtrar</param>
        /// <param name="dataFim">Data de fim opcional para filtrar</param>
        /// <returns>Lista de leituras RFID da moto</returns>
        [HttpGet("Historico/Moto/{motoId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<LeituraRFIDDTO>>> GetHistoricoMoto(
            int motoId,
            [FromQuery] DateTime? dataInicio = null,
            [FromQuery] DateTime? dataFim = null)
        {
            var moto = await _context.Motos.FindAsync(motoId);
            if (moto == null)
            {
                return NotFound($"Moto ID {motoId} não encontrada");
            }

            var query = _context.LeiturasRFID
                .Include(l => l.PontoLeitura)
                .Where(l => l.MotoId == motoId)
                .AsQueryable();

            if (dataInicio.HasValue)
                query = query.Where(l => l.DataHoraLeitura >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(l => l.DataHoraLeitura <= dataFim.Value);

            query = query.OrderByDescending(l => l.DataHoraLeitura);

            var leituras = await query.ToListAsync();

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

        /// <summary>
        /// Obtém o histórico de leituras RFID em um ponto de leitura específico
        /// </summary>
        /// <param name="pontoId">ID do ponto de leitura</param>
        /// <param name="dataInicio">Data de início opcional para filtrar</param>
        /// <param name="dataFim">Data de fim opcional para filtrar</param>
        /// <returns>Lista de leituras RFID no ponto de leitura</returns>
        [HttpGet("Historico/PontoLeitura/{pontoId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<LeituraRFIDDTO>>> GetHistoricoPontoLeitura(
            int pontoId,
            [FromQuery] DateTime? dataInicio = null,
            [FromQuery] DateTime? dataFim = null)
        {
            var pontoLeitura = await _context.PontosLeitura.FindAsync(pontoId);
            if (pontoLeitura == null)
            {
                return NotFound($"Ponto de leitura ID {pontoId} não encontrado");
            }

            var query = _context.LeiturasRFID
                .Include(l => l.Moto)
                .Where(l => l.PontoLeituraId == pontoId)
                .AsQueryable();

            if (dataInicio.HasValue)
                query = query.Where(l => l.DataHoraLeitura >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(l => l.DataHoraLeitura <= dataFim.Value);

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
                PontoLeituraNome = pontoLeitura.Nome,
                DataHoraLeitura = l.DataHoraLeitura,
                Observacao = l.Observacao
            }));
        }

        /// <summary>
        /// Obtém a localização atual de todas as motos de uma filial
        /// </summary>
        /// <param name="filialId">ID da filial</param>
        /// <returns>Lista de motos com suas localizações atuais</returns>
        [HttpGet("Localizacao/Filial/{filialId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<LocalizacaoMotoDTO>>> GetLocalizacaoMotosFilial(int filialId)
        {
            var filial = await _context.Filiais.FindAsync(filialId);
            if (filial == null)
            {
                return NotFound($"Filial ID {filialId} não encontrada");
            }

            var motos = await _context.Motos
                .Include(m => m.PontoLeituraAtual)
                .Where(m => m.FilialId == filialId && m.Ativa)
                .ToListAsync();

            return Ok(motos.Select(m => new LocalizacaoMotoDTO
            {
                MotoId = m.Id,
                Placa = m.Placa,
                Modelo = m.Modelo,
                Cor = m.Cor,
                TagRFID = m.TagRFID,
                PontoLeituraId = m.PontoLeituraAtualId,
                PontoLeituraNome = m.PontoLeituraAtual?.Nome,
                PontoLeituraLocalizacao = m.PontoLeituraAtual?.Localizacao,
                PosicaoX = m.PontoLeituraAtual?.PosicaoX ?? 0,
                PosicaoY = m.PontoLeituraAtual?.PosicaoY ?? 0,
                UltimaAtualizacao = m.UltimaAtualizacao
            }));
        }

        /// <summary>
        /// Obtém a contagem de motos em cada ponto de leitura de uma filial
        /// </summary>
        /// <param name="filialId">ID da filial</param>
        /// <returns>Lista de pontos de leitura com a contagem de motos em cada um</returns>
        [HttpGet("Contagem/Filial/{filialId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<ContagemPontoDTO>>> GetContagemPontosFilial(int filialId)
        {
            var filial = await _context.Filiais.FindAsync(filialId);
            if (filial == null)
            {
                return NotFound($"Filial ID {filialId} não encontrada");
            }

            var pontosLeitura = await _context.PontosLeitura
                .Where(p => p.FilialId == filialId && p.Ativo)
                .ToListAsync();

            var result = new List<ContagemPontoDTO>();

            foreach (var ponto in pontosLeitura)
            {
                var quantidadeMotos = await _context.Motos
                    .CountAsync(m => m.PontoLeituraAtualId == ponto.Id && m.Ativa);

                result.Add(new ContagemPontoDTO
                {
                    PontoLeituraId = ponto.Id,
                    Nome = ponto.Nome,
                    Localizacao = ponto.Localizacao,
                    PosicaoX = ponto.PosicaoX,
                    PosicaoY = ponto.PosicaoY,
                    QuantidadeMotos = quantidadeMotos
                });
            }

            return Ok(result);
        }
    }

    // Classes para request/response da API

    public class RegistroLeituraRequest
    {
        [Required]
        [StringLength(50)]
        public string TagRFID { get; set; }

        [Required]
        public int PontoLeituraId { get; set; }

        public string? Observacao { get; set; }
    }

    public class RegistroLeituraResponse
    {
        public int LeituraId { get; set; }
        public DateTime DataHoraLeitura { get; set; }
        public MotoInfo Moto { get; set; }
        public PontoLeituraInfo PontoLeitura { get; set; }
        public FilialInfo Filial { get; set; }
    }

    public class MotoInfo
    {
        public int Id { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public string Cor { get; set; }
        public string TagRFID { get; set; }
    }

    public class PontoLeituraInfo
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Localizacao { get; set; }
        public double PosicaoX { get; set; }
        public double PosicaoY { get; set; }
    }

    public class FilialInfo
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CodigoFilial { get; set; }
    }

    public class LocalizacaoMotoDTO
    {
        public int MotoId { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public string Cor { get; set; }
        public string TagRFID { get; set; }
        public int? PontoLeituraId { get; set; }
        public string PontoLeituraNome { get; set; }
        public string PontoLeituraLocalizacao { get; set; }
        public double PosicaoX { get; set; }
        public double PosicaoY { get; set; }
        public DateTime? UltimaAtualizacao { get; set; }
    }

    public class ContagemPontoDTO
    {
        public int PontoLeituraId { get; set; }
        public string Nome { get; set; }
        public string Localizacao { get; set; }
        public double PosicaoX { get; set; }
        public double PosicaoY { get; set; }
        public int QuantidadeMotos { get; set; }
    }
}
