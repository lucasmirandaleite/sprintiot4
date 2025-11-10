using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MottuRFID.Application.DTOs
{
    public class FilialDTO
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; }
        
        [StringLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres")]
        public string Endereco { get; set; }
        
        [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres")]
        public string Cidade { get; set; }
        
        [StringLength(50, ErrorMessage = "O estado deve ter no máximo 50 caracteres")]
        public string Estado { get; set; }
        
        [StringLength(50, ErrorMessage = "O país deve ter no máximo 50 caracteres")]
        public string Pais { get; set; }
        
        [Required(ErrorMessage = "O código da filial é obrigatório")]
        [StringLength(20, ErrorMessage = "O código da filial deve ter no máximo 20 caracteres")]
        public string CodigoFilial { get; set; }
        
        public int QuantidadeMotos { get; set; }
        
        public int QuantidadePontosLeitura { get; set; }
    }
}
