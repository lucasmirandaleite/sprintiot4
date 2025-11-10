using System;
using System.Collections.Generic;

namespace MottuRFID.Domain.Entities
{
    public class PontoLeitura
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Localizacao { get; set; }
        public int FilialId { get; set; }
        public Filial Filial { get; set; }
        public double PosicaoX { get; set; }
        public double PosicaoY { get; set; }
        public bool Ativo { get; set; }
        public ICollection<Moto> MotosAtuais { get; set; }
        public ICollection<LeituraRFID> Leituras { get; set; }
    }
}
