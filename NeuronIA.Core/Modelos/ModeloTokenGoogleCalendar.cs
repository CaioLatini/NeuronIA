using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeuronIA.Core.Modelos;

public class ModeloTokenGoogleCalendar
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required int UsuarioId { get; set; }
    
    public required string RefreshTokenGoogleCalendar { get; set; }

    // Propriedade de Navegação (necessária para o EF entender a relação corretamente)
    [ForeignKey("UsuarioId")]
    public virtual ModeloUsuario? ModeloUsuario { get; set; }
}