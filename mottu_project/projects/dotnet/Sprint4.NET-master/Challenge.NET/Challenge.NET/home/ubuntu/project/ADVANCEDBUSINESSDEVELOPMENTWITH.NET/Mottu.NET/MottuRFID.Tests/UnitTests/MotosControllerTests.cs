using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuRFID.API.Controllers;
using MottuRFID.Application.DTOs;
using MottuRFID.Domain.Entities;
using MottuRFID.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MottuRFID.Tests.UnitTests
{
    /// <summary>
    /// Testes unitários para o MotosController
    /// </summary>
    public class MotosControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly MotosController _controller;

        public MotosControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new MotosController(_context);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var filial = new Filial
            {
                Id = 1,
                Nome = "Filial Teste",
                Endereco = "Rua Teste, 123",
                Cidade = "São Paulo",
                Estado = "SP",
                Pais = "Brasil",
                CodigoFilial = "FIL001"
            };

            var pontoLeitura = new PontoLeitura
            {
                Id = 1,
                Nome = "Ponto Teste",
                Descricao = "Ponto de leitura para testes",
                Localizacao = "Entrada Principal",
                FilialId = 1,
                PosicaoX = 10.0,
                PosicaoY = 20.0,
                Ativo = true
            };

            var moto = new Moto
            {
                Id = 1,
                Placa = "ABC-1234",
                Modelo = "Honda CG 160",
                Cor = "Vermelha",
                NumeroSerie = "123456789",
                TagRFID = "RF001",
                FilialId = 1,
                PontoLeituraAtualId = 1,
                UltimaAtualizacao = DateTime.Now,
                Ativa = true
            };

            _context.Filiais.Add(filial);
            _context.PontosLeitura.Add(pontoLeitura);
            _context.Motos.Add(moto);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetMotos_ReturnsOkResult_WithListOfMotos()
        {
            // Act
            var result = await _controller.GetMotos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var motos = Assert.IsAssignableFrom<IEnumerable<MotoDTO>>(okResult.Value);
            Assert.Single(motos);
        }

        [Fact]
        public async Task GetMoto_WithValidId_ReturnsOkResult()
        {
            // Act
            var result = await _controller.GetMoto(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var moto = Assert.IsType<MotoDTO>(okResult.Value);
            Assert.Equal("ABC-1234", moto.Placa);
        }

        [Fact]
        public async Task GetMoto_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetMoto(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostMoto_WithValidData_ReturnsCreatedResult()
        {
            // Arrange
            var novaMotoDto = new MotoDTO
            {
                Placa = "XYZ-9876",
                Modelo = "Yamaha Factor 125",
                Cor = "Azul",
                NumeroSerie = "987654321",
                TagRFID = "RF002",
                FilialId = 1,
                Ativa = true
            };

            // Act
            var result = await _controller.PostMoto(novaMotoDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var moto = Assert.IsType<MotoDTO>(createdResult.Value);
            Assert.Equal("XYZ-9876", moto.Placa);
        }

        [Fact]
        public async Task PostMoto_WithDuplicatePlaca_ReturnsBadRequest()
        {
            // Arrange
            var motoComPlacaDuplicada = new MotoDTO
            {
                Placa = "ABC-1234", // Placa já existe
                Modelo = "Yamaha Factor 125",
                Cor = "Azul",
                NumeroSerie = "987654321",
                TagRFID = "RF003",
                FilialId = 1,
                Ativa = true
            };

            // Act
            var result = await _controller.PostMoto(motoComPlacaDuplicada);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task PutMoto_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var motoAtualizada = new MotoDTO
            {
                Id = 1,
                Placa = "ABC-1234",
                Modelo = "Honda CG 160 Atualizada",
                Cor = "Verde",
                NumeroSerie = "123456789",
                TagRFID = "RF001",
                FilialId = 1,
                Ativa = true
            };

            // Act
            var result = await _controller.PutMoto(1, motoAtualizada);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verificar se foi atualizada
            var motoNoBanco = await _context.Motos.FindAsync(1);
            Assert.Equal("Honda CG 160 Atualizada", motoNoBanco.Modelo);
            Assert.Equal("Verde", motoNoBanco.Cor);
        }

        [Fact]
        public async Task DeleteMoto_WithValidId_ReturnsNoContent()
        {
            // Act
            var result = await _controller.DeleteMoto(1);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verificar se foi removida
            var motoNoBanco = await _context.Motos.FindAsync(1);
            Assert.Null(motoNoBanco);
        }

        [Fact]
        public async Task GetMotoByTag_WithValidTag_ReturnsOkResult()
        {
            // Act
            var result = await _controller.GetMotoByTag("RF001");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var moto = Assert.IsType<MotoDTO>(okResult.Value);
            Assert.Equal("ABC-1234", moto.Placa);
        }

        [Fact]
        public async Task GetMotoByTag_WithInvalidTag_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetMotoByTag("RF999");

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

