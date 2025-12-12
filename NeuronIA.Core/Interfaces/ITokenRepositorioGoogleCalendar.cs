namespace NeuronIA.Core.Interfaces;

public interface ITokenRepositorioGoogleCalendar
{
    // Salva o Refresh Token associado a um ID de usuário
    Task SalvarRefreshTokenGoogleCalendarAsync(int usuarioId, string refreshTokenGoogle);

    // Busca o Refresh Token para uso na API do Google
    Task<string> PegarRefreshTokenGoogleCalendarAsync(int usuarioId);
}
