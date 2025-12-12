using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeuronIA.Core.Modelos;
public class ModeloUsuario
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public required string SenhaHash { get; set; }
}