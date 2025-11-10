using MottuRFID.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MottuRFID.Application.Services
{
    public class MotoStatusService
    {
        // Simulação de banco de dados em memória para o status das motos
        private readonly Dictionary<int, MotoStatusDTO> _motoStatuses = new Dictionary<int, MotoStatusDTO>();
        private readonly Random _random = new Random();

        public MotoStatusService()
        {
            // Inicializa com algumas motos de exemplo
            for (int i = 1; i <= 5; i++)
            {
                _motoStatuses.Add(i, new MotoStatusDTO
                {
                    MotoId = i,
                    Placa = $"ABC-{1000 + i}",
                    Status = "Parada",
                    Localizacao = "Pátio Principal",
                    X = _random.Next(50, 950),
                    Y = _random.Next(50, 450),
                    LastUpdated = DateTime.UtcNow.ToString("o")
                });
            }
        }

        public IEnumerable<MotoStatusDTO> GetAllStatuses()
        {
            return _motoStatuses.Values;
        }

        public void UpdateStatusFromVision(VisionDetectionDTO detection)
        {
            // Lógica de atualização simplificada para o MVP:
            // A detecção de Visão Computacional apenas atualiza o status de uma moto aleatória
            // e sua localização (simulando a posição no pátio).

            if (_motoStatuses.Any())
            {
                // Simula que a detecção afeta uma moto aleatória
                var motoIdToUpdate = _random.Next(1, _motoStatuses.Count + 1);
                var moto = _motoStatuses[motoIdToUpdate];

                moto.Status = detection.MotoCount > 0 ? "Detectada por Visão" : "Parada";
                moto.Localizacao = detection.Source;
                moto.X = _random.Next(50, 950); // Nova posição simulada
                moto.Y = _random.Next(50, 450); // Nova posição simulada
                moto.LastUpdated = DateTime.UtcNow.ToString("o");
            }
        }
    }
}
