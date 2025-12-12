using System.ComponentModel.DataAnnotations;

namespace NeuronIA.Core.DTOs
{
    public class CriarEventoCalendar
    {
        [Required]
        public required int UsuarioId { get; set; }
        [Required]
        public required string Summary { get; set; } // Título do evento
        public string? Location { get; set; }
        public string? Description { get; set; }
        [Required]
        public required DateTime StartTime { get; set; }
        [Required]
        public required DateTime EndTime { get; set; }
        public string TimeZone { get; set; } = "America/Sao_Paulo"; // Fuso horário padrão para o evento
    }

    public class EditarEventoCalendar
    {
        [Required]
        public required int UsuarioId { get; set; }
        [Required]
        public required int EventId { get; set; } // ID único do evento retornado pela API do Google
        public required string Summary { get; set; } // Título do evento
        public string? Location { get; set; }
        public string? Description { get; set; }
        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }
        public string TimeZone { get; set; } = "America/Sao_Paulo"; // Fuso horário padrão para o evento
    }

    public class ListarEventosCalendar
    {
        [Required]
        public required int UsuarioId { get; set; }
        [Required]
        public required DateTime Inicio { get; set; }
        [Required]
        public required DateTime Fim { get; set; }
    }

    public class ApagarEventoCalendar
    {
        [Required]
        public required int UsuarioId { get; set; }
        [Required]
        public required string EventId { get; set; } // ID único do evento retornado pela API do Google
    }
}