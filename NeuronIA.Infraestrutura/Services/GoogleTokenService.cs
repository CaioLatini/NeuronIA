using NeuronIA.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace NeuronIA.Infraestrutura.Services;

public class GoogleTokenService : IGoogleTokenService
{
    private readonly ITokenRepositorioGoogleCalendar _tokenRepositorioGoogleCalendar;
    private readonly IConfiguration _config;
    private readonly HttpClient _http;

    public GoogleTokenService(ITokenRepositorioGoogleCalendar tokenRepositorioGoogleCalendar, IConfiguration config, HttpClient http)
    {
        _tokenRepositorioGoogleCalendar = tokenRepositorioGoogleCalendar;
        _config = config;
        _http = http;
    }

    public async Task<string> ObterTokenAsync(int usuarioId)
    {
        string refreshToken = await _tokenRepositorioGoogleCalendar.PegarRefreshTokenGoogleCalendarAsync(usuarioId);

        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new Exception($"Refresh Token não encontrado para o usuário {usuarioId}.");
        }

        var tokenUrl = "https://oauth2.googleapis.com/token";
        var values = new Dictionary<string, string>
        {
            { "refresh_token", refreshToken }, 
            { "client_id", _config["GoogleCalendarApi:ClientId"] },
            { "client_secret", _config["GoogleCalendarApi:ClientSecret"] },
            { "grant_type", "refresh_token" }
        };


        var content = new FormUrlEncodedContent(values);
        var response = await _http.PostAsync(tokenUrl, content);
        
        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        
        string newAccessToken = root.TryGetProperty("access_token", out var at) ? at.GetString() : null;

        return newAccessToken;
    }
}