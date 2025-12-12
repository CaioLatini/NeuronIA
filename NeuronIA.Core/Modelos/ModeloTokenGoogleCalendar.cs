using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeuronIA.Core.Modelos;

public class ModeloTokenGoogleCalendar
{
    [Key]
    [ForeignKey("ModeloUsuario")]
    public required int IdUsuario { get; set; }
    public required string RefreshTokenGoogleCalendar { get; set; }
}
