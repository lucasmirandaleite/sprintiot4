using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MottuRFID.Application.DTOs
{
    public class PontoLeituraDTO
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; }
        
        [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
        public string Descricao { get; set; }
        
        [StringLength(100, ErrorMessage = "A localização deve ter no máximo 100 caracteres")]
        public string Localizacao { get; set; }
        
        [Required(ErrorMessage = "A filial é obrigatória")]
        public int FilialId { get; set; }
        
        public string FilialNome { get; set; }
        
        [Required(ErrorMessage = "A posição X é obrigatória")]
        public double PosicaoX { get; set; }
        
        [Required(ErrorMessage = "A posição Y é obrigatória")]
        public double PosicaoY { get; set; }
        
        public bool Ativo { get; set; }
        
        public int QuantidadeMotosAtuais { get; set; }
    }
}
