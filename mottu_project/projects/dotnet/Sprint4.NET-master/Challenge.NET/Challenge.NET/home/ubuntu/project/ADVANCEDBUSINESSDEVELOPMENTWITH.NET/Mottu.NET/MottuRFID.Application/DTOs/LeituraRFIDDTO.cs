using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MottuRFID.Application.DTOs
{
    public class LeituraRFIDDTO
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "A tag RFID é obrigatória")]
        [StringLength(50, ErrorMessage = "A tag RFID deve ter no máximo 50 caracteres")]
        public string TagRFID { get; set; }
        
        public int MotoId { get; set; }
        
        public string MotoPlaca { get; set; }
        
        public string MotoModelo { get; set; }
        
        [Required(ErrorMessage = "O ponto de leitura é obrigatório")]
        public int PontoLeituraId { get; set; }
        
        public string PontoLeituraNome { get; set; }
        
        [Required(ErrorMessage = "A data e hora da leitura são obrigatórias")]
        public DateTime DataHoraLeitura { get; set; }
        
        [StringLength(200, ErrorMessage = "A observação deve ter no máximo 200 caracteres")]
        public string Observacao { get; set; }
    }
}
