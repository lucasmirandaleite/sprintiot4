using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MottuRFID.Application.DTOs
{
    public class MotoDTO
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "A placa é obrigatória")]
        [StringLength(10, ErrorMessage = "A placa deve ter no máximo 10 caracteres")]
        public string Placa { get; set; }
        
        [Required(ErrorMessage = "O modelo é obrigatório")]
        [StringLength(100, ErrorMessage = "O modelo deve ter no máximo 100 caracteres")]
        public string Modelo { get; set; }
        
        [StringLength(50, ErrorMessage = "A cor deve ter no máximo 50 caracteres")]
        public string Cor { get; set; }
        
        [StringLength(50, ErrorMessage = "O número de série deve ter no máximo 50 caracteres")]
        public string NumeroSerie { get; set; }
        
        [Required(ErrorMessage = "A tag RFID é obrigatória")]
        [StringLength(50, ErrorMessage = "A tag RFID deve ter no máximo 50 caracteres")]
        public string TagRFID { get; set; }
        
        [Required(ErrorMessage = "A filial é obrigatória")]
        public int FilialId { get; set; }
        
        public string FilialNome { get; set; }
        
        public int? PontoLeituraAtualId { get; set; }
        
        public string PontoLeituraAtualNome { get; set; }
        
        public DateTime UltimaAtualizacao { get; set; }
        
        public bool Ativa { get; set; }
    }
}
