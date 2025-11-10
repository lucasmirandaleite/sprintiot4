using System;
using System.Collections.Generic;

namespace MottuRFID.Domain.Entities
{
    public class LeituraRFID
    {
        public int Id { get; set; }
        public string TagRFID { get; set; }
        public int MotoId { get; set; }
        public Moto Moto { get; set; }
        public int PontoLeituraId { get; set; }
        public PontoLeitura PontoLeitura { get; set; }
        public DateTime DataHoraLeitura { get; set; }
        public string Observacao { get; set; }
    }
}
