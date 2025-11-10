using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuRFID.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using Microsoft.ML.Trainers;

namespace MottuRFID.API.Controllers
{
    /// <summary>
    /// Controller para funcionalidades de Machine Learning
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MLController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MLContext _mlContext;

        public MLController(ApplicationDbContext context)
        {
            _context = context;
            _mlContext = new MLContext(seed: 0);
        }

        /// <summary>
        /// Prediz o próximo ponto de leitura baseado no histórico de uma moto
        /// </summary>
        /// <param name="motoId">ID da moto</param>
        /// <returns>Predição do próximo ponto de leitura</returns>
        [HttpGet("PredictNextLocation/{motoId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<LocationPredictionResult>> PredictNextLocation(int motoId)
        {
            var moto = await _context.Motos.FindAsync(motoId);
            if (moto == null)
            {
                return NotFound($"Moto ID {motoId} não encontrada");
            }

            // Obter histórico de leituras da moto
            var historico = await _context.LeiturasRFID
                .Include(l => l.PontoLeitura)
                .Where(l => l.MotoId == motoId)
                .OrderBy(l => l.DataHoraLeitura)
                .Select(l => new LocationData
                {
                    PontoLeituraId = l.PontoLeituraId,
                    HoraDia = l.DataHoraLeitura.Hour,
                    DiaSemana = (int)l.DataHoraLeitura.DayOfWeek,
                    PosicaoX = l.PontoLeitura.PosicaoX,
                    PosicaoY = l.PontoLeitura.PosicaoY
                })
                .ToListAsync();

            if (historico.Count < 5)
            {
                return BadRequest("Histórico insuficiente para predição (mínimo 5 registros)");
            }

            try
            {
                // Preparar dados para treinamento
                var trainingData = PrepareTrainingData(historico);
                var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

                // Criar pipeline de ML
                var pipeline = _mlContext.Transforms.Categorical.OneHotEncoding("HoraDiaEncoded", "HoraDia")
                    .Append(_mlContext.Transforms.Categorical.OneHotEncoding("DiaSemanaEncoded", "DiaSemana"))
                    .Append(_mlContext.Transforms.Concatenate("Features", "HoraDiaEncoded", "DiaSemanaEncoded", "PosicaoX", "PosicaoY"))
                    .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: "NextPontoLeituraId", maximumNumberOfIterations: 100));

                // Treinar modelo
                var model = pipeline.Fit(dataView);

                // Fazer predição
                var predictionEngine = _mlContext.Model.CreatePredictionEngine<LocationTrainingData, LocationPrediction>(model);
                
                var ultimaLeitura = historico.Last();
                var inputData = new LocationTrainingData
                {
                    HoraDia = DateTime.Now.Hour,
                    DiaSemana = (int)DateTime.Now.DayOfWeek,
                    PosicaoX = ultimaLeitura.PosicaoX,
                    PosicaoY = ultimaLeitura.PosicaoY
                };

                var prediction = predictionEngine.Predict(inputData);

                // Encontrar o ponto de leitura mais próximo da predição
                var pontosPreditos = await _context.PontosLeitura
                    .Where(p => p.FilialId == moto.FilialId)
                    .ToListAsync();

                var pontoMaisProximo = pontosPreditos
                    .OrderBy(p => Math.Abs(p.Id - prediction.NextPontoLeituraId))
                    .First();

                var result = new LocationPredictionResult
                {
                    MotoId = motoId,
                    MotoPlaca = moto.Placa,
                    PontoLeituraPredito = new PontoLeituraInfo
                    {
                        Id = pontoMaisProximo.Id,
                        Nome = pontoMaisProximo.Nome,
                        Localizacao = pontoMaisProximo.Localizacao,
                        PosicaoX = pontoMaisProximo.PosicaoX,
                        PosicaoY = pontoMaisProximo.PosicaoY
                    },
                    Confianca = Math.Min(95.0, Math.Max(60.0, 100.0 - Math.Abs(pontoMaisProximo.Id - prediction.NextPontoLeituraId) * 5)),
                    DataHoraPredição = DateTime.Now,
                    HistoricoUtilizado = historico.Count
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na predição: {ex.Message}");
            }
        }

        /// <summary>
        /// Analisa padrões de movimento das motos em uma filial
        /// </summary>
        /// <param name="filialId">ID da filial</param>
        /// <returns>Análise de padrões de movimento</returns>
        [HttpGet("AnalyzeMovementPatterns/{filialId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MovementAnalysisResult>> AnalyzeMovementPatterns(int filialId)
        {
            var filial = await _context.Filiais.FindAsync(filialId);
            if (filial == null)
            {
                return NotFound($"Filial ID {filialId} não encontrada");
            }

            // Obter dados de movimento dos últimos 30 dias
            var dataLimite = DateTime.Now.AddDays(-30);
            var movimentos = await _context.LeiturasRFID
                .Include(l => l.Moto)
                .Include(l => l.PontoLeitura)
                .Where(l => l.Moto.FilialId == filialId && l.DataHoraLeitura >= dataLimite)
                .OrderBy(l => l.DataHoraLeitura)
                .ToListAsync();

            if (!movimentos.Any())
            {
                return BadRequest("Dados insuficientes para análise");
            }

            // Análise de padrões por hora
            var padroesPorHora = movimentos
                .GroupBy(m => m.DataHoraLeitura.Hour)
                .Select(g => new HourlyPattern
                {
                    Hora = g.Key,
                    QuantidadeMovimentos = g.Count(),
                    MotosUnicas = g.Select(m => m.MotoId).Distinct().Count()
                })
                .OrderBy(p => p.Hora)
                .ToList();

            // Análise de pontos mais utilizados
            var pontosPopulares = movimentos
                .GroupBy(m => new { m.PontoLeituraId, m.PontoLeitura.Nome })
                .Select(g => new PopularLocation
                {
                    PontoLeituraId = g.Key.PontoLeituraId,
                    Nome = g.Key.Nome,
                    QuantidadeVisitas = g.Count(),
                    MotosUnicas = g.Select(m => m.MotoId).Distinct().Count()
                })
                .OrderByDescending(p => p.QuantidadeVisitas)
                .Take(10)
                .ToList();

            // Análise de eficiência (tempo médio entre movimentos)
            var temposMovimento = new List<double>();
            var movimentosPorMoto = movimentos.GroupBy(m => m.MotoId);

            foreach (var motoMovimentos in movimentosPorMoto)
            {
                var movimentosOrdenados = motoMovimentos.OrderBy(m => m.DataHoraLeitura).ToList();
                for (int i = 1; i < movimentosOrdenados.Count; i++)
                {
                    var tempo = (movimentosOrdenados[i].DataHoraLeitura - movimentosOrdenados[i - 1].DataHoraLeitura).TotalMinutes;
                    if (tempo > 0 && tempo < 480) // Ignorar tempos muito longos (mais de 8 horas)
                    {
                        temposMovimento.Add(tempo);
                    }
                }
            }

            var result = new MovementAnalysisResult
            {
                FilialId = filialId,
                FilialNome = filial.Nome,
                PeriodoAnalise = $"{dataLimite:dd/MM/yyyy} - {DateTime.Now:dd/MM/yyyy}",
                TotalMovimentos = movimentos.Count,
                MotosAnalisadas = movimentos.Select(m => m.MotoId).Distinct().Count(),
                PadroesPorHora = padroesPorHora,
                PontosPopulares = pontosPopulares,
                TempoMedioEntreMovimentos = temposMovimento.Any() ? Math.Round(temposMovimento.Average(), 2) : 0,
                HoraPico = padroesPorHora.OrderByDescending(p => p.QuantidadeMovimentos).First().Hora,
                DataAnalise = DateTime.Now
            };

            return Ok(result);
        }

        private List<LocationTrainingData> PrepareTrainingData(List<LocationData> historico)
        {
            var trainingData = new List<LocationTrainingData>();

            for (int i = 0; i < historico.Count - 1; i++)
            {
                trainingData.Add(new LocationTrainingData
                {
                    HoraDia = historico[i].HoraDia,
                    DiaSemana = historico[i].DiaSemana,
                    PosicaoX = historico[i].PosicaoX,
                    PosicaoY = historico[i].PosicaoY,
                    NextPontoLeituraId = historico[i + 1].PontoLeituraId
                });
            }

            return trainingData;
        }
    }

    // Classes para ML.NET
    public class LocationData
    {
        public int PontoLeituraId { get; set; }
        public int HoraDia { get; set; }
        public int DiaSemana { get; set; }
        public double PosicaoX { get; set; }
        public double PosicaoY { get; set; }
    }

    public class LocationTrainingData
    {
        public int HoraDia { get; set; }
        public int DiaSemana { get; set; }
        public double PosicaoX { get; set; }
        public double PosicaoY { get; set; }
        public float NextPontoLeituraId { get; set; }
    }

    public class LocationPrediction
    {
        [ColumnName("Score")]
        public float NextPontoLeituraId { get; set; }
    }

    // Classes para resposta da API
    public class LocationPredictionResult
    {
        public int MotoId { get; set; }
        public string MotoPlaca { get; set; }
        public PontoLeituraInfo PontoLeituraPredito { get; set; }
        public double Confianca { get; set; }
        public DateTime DataHoraPredição { get; set; }
        public int HistoricoUtilizado { get; set; }
    }

    public class MovementAnalysisResult
    {
        public int FilialId { get; set; }
        public string FilialNome { get; set; }
        public string PeriodoAnalise { get; set; }
        public int TotalMovimentos { get; set; }
        public int MotosAnalisadas { get; set; }
        public List<HourlyPattern> PadroesPorHora { get; set; }
        public List<PopularLocation> PontosPopulares { get; set; }
        public double TempoMedioEntreMovimentos { get; set; }
        public int HoraPico { get; set; }
        public DateTime DataAnalise { get; set; }
    }

    public class HourlyPattern
    {
        public int Hora { get; set; }
        public int QuantidadeMovimentos { get; set; }
        public int MotosUnicas { get; set; }
    }

    public class PopularLocation
    {
        public int PontoLeituraId { get; set; }
        public string Nome { get; set; }
        public int QuantidadeVisitas { get; set; }
        public int MotosUnicas { get; set; }
    }
}

