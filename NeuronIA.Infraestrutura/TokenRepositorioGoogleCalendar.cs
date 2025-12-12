using NeuronIA.Core.Interfaces;
using NeuronIA.Infraestrutura.Dados;
using NeuronIA.Core.Modelos;

namespace NeuronIA.Infraestrutura;

public class TokenRepositorioGoogleCalendar : ITokenRepositorioGoogleCalendar
{

    private readonly NeuronIADbContext _context;
    public TokenRepositorioGoogleCalendar(NeuronIADbContext context)
    {
        _context = context;
    }

    public async Task SalvarRefreshTokenGoogleCalendarAsync(int usuarioId, string refreshTokenGoogleCalendar)
    {
        var usuarioExiste = await _context.Usuarios.FindAsync(usuarioId);
        if(usuarioExiste != null)
        {
            var token = await _context.GoogleCalendarTokens.FindAsync(usuarioId);
            if(token != null)
            {
                token.RefreshTokenGoogleCalendar = refreshTokenGoogleCalendar;
                await _context.SaveChangesAsync();
            } else
            {
                var novoToken = new ModeloTokenGoogleCalendar
                {
                    UsuarioId = usuarioId,
                    RefreshTokenGoogleCalendar = refreshTokenGoogleCalendar
                };
                _context.GoogleCalendarTokens.Add(novoToken);
                await _context.SaveChangesAsync();
            }
            
        } else throw new Exception("Usuário não encontrado");
    }

    public async Task<string> PegarRefreshTokenGoogleCalendarAsync(int usuarioId)
    {
        var usuario = await _context.GoogleCalendarTokens.FindAsync(usuarioId);
        if(usuario != null)
        {
            return usuario.RefreshTokenGoogleCalendar;
        } else throw new Exception("Usuário não encontrado");
    }
}
