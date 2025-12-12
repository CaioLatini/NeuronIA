using NeuronIA.Core.Interfaces;
using NeuronIA.Infraestrutura.Dados;

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
        var usuario = await _context.GoogleCalendarTokens.FindAsync(usuarioId);
        if(usuario != null)
        {
            usuario.RefreshTokenGoogleCalendar = refreshTokenGoogleCalendar;
            await _context.SaveChangesAsync();
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
