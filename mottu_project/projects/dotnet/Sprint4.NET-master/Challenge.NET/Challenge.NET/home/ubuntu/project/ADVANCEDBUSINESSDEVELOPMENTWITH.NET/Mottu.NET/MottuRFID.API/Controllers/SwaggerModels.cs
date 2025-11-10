using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MottuRFID.API.Controllers
{
    /// <summary>
    /// Modelos para documentação Swagger
    /// </summary>
    public class SwaggerModels
    {
        /// <summary>
        /// Exemplo de modelo para documentação
        /// </summary>
        public class ExemploModel
        {
            /// <summary>
            /// Identificador único
            /// </summary>
            [Required]
            public int Id { get; set; }

            /// <summary>
            /// Nome do item
            /// </summary>
            [Required]
            [StringLength(100)]
            public string Nome { get; set; }

            /// <summary>
            /// Descrição do item
            /// </summary>
            [StringLength(500)]
            public string Descricao { get; set; }
        }
    }
}
