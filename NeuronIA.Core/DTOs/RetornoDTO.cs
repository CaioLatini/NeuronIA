using System.ComponentModel.DataAnnotations;

namespace NeuronIA.Core.DTOs
{
    public class RetornoDTO
    {
        [Required]
        public required bool Sucesso { get; set; } 
        public string? Mensagem { get; set; } 
        public object? Dados { get; set; } 
    }
}