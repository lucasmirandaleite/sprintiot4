using System;
using System.Collections.Generic;

namespace MottuRFID.Domain.Entities
{
    public class Filial
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string CodigoFilial { get; set; }
        public ICollection<Moto> Motos { get; set; }
        public ICollection<PontoLeitura> PontosLeitura { get; set; }
    }
}
