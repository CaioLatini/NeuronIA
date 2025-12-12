using Microsoft.EntityFrameworkCore;
using NeuronIA.Core.Modelos;

namespace NeuronIA.Infraestrutura.Dados
{
    public class NeuronIADbContext : DbContext
    {
        // Construtor obrigatório para Injeção de Dependência
        public NeuronIADbContext(DbContextOptions<NeuronIADbContext> options) : base(options)
        {
        }
        
        public DbSet<ModeloTokenGoogleCalendar> GoogleCalendarTokens { get; set; }
        public DbSet<ModeloUsuario> Usuarios { get; set; }
    }
}