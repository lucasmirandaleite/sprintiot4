using System;
using System.Collections.Generic;

namespace MottuRFID.Domain.Entities
{
    public class Moto
    {
        public int Id { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public string Cor { get; set; }
        public string NumeroSerie { get; set; }
        public string TagRFID { get; set; }
        public int FilialId { get; set; }
        public Filial Filial { get; set; }
        public int? PontoLeituraAtualId { get; set; }
        public PontoLeitura PontoLeituraAtual { get; set; }
        public DateTime UltimaAtualizacao { get; set; }
        public bool Ativa { get; set; }
        public ICollection<LeituraRFID> Leituras { get; set; }
    }
}
