using System.ComponentModel.DataAnnotations;

namespace NeuronIA.Core.DTOs
{
    public class UsuarioCriacaoDTO
    {
        [Required]
        public string Nome { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Senha { get; set; }
    }

    public class UsuarioRespostaDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
    }
    public class UsuarioAtualizacaoDTO
    {
        [Required]
        public int Id { get; set; }
        public string Nome { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}