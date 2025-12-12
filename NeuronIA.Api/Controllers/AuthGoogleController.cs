using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Web;
using NeuronIA.Core.DTOs; 
using NeuronIA.Core.Interfaces; 

[ApiController]
[Route("api/[controller]")]
public class GoogleOAuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly HttpClient _http;
    private readonly ITokenRepositorioGoogleCalendar _tokenRepositorio; 

    public GoogleOAuthController(IConfiguration config, ITokenRepositorioGoogleCalendar tokenRepositorio)
    {
        _config = config;
        _http = new HttpClient();
        _tokenRepositorio = tokenRepositorio;
    }

    // 1) --------------------------------------------
    // Gera o link de autorização para o usuário (ID do usuário é o "state")
    // ------------------------------------------------
    [HttpGet("Login/{usuarioId}")]
    public IActionResult Login(int usuarioId)
    {
        string clientId = _config["GoogleCalendarApi:ClientId"];
        string redirectUri = _config["GoogleCalendarApi:RedirectUri"];
        string scope = _config["GoogleCalendarApi:Scopes"];
        string state = usuarioId.ToString();

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(redirectUri) || string.IsNullOrEmpty(scope))
        {
            return BadRequest(new RetornoDTO { Sucesso = false, Mensagem = "Credenciais do Google não configuradas." });
        }

        var url = "https://accounts.google.com/o/oauth2/v2/auth" +
            "?response_type=code" +
            $"&client_id={clientId}" +
            $"&redirect_uri={HttpUtility.UrlEncode(redirectUri)}" +
            $"&scope={HttpUtility.UrlEncode(scope)}" +
            $"&state={HttpUtility.UrlEncode(state)}" +
            "&access_type=offline" + 
            "&prompt=consent"; 

        return Redirect(url);
    }

    // 2) --------------------------------------------
    // Google redireciona p/ este endpoint com o "code" e o "state"
    // ------------------------------------------------
    [HttpGet("/oauth2callback")]
    public async Task<IActionResult> OAuthCallback([FromQuery] string code, [FromQuery] string state)
    {
        // 1. Validação
        if (string.IsNullOrEmpty(code))
            return BadRequest(new RetornoDTO { Sucesso = false, Mensagem = "Código de autorização não recebido." });

        if (string.IsNullOrEmpty(state) || !int.TryParse(state, out int usuarioId))
            return BadRequest(new RetornoDTO { Sucesso = false, Mensagem = "ID do Usuário não recebido ou inválido." });

        string tokenUrl = "https://oauth2.googleapis.com/token";

        // 2. Prepara a requisição POST para troca de código
        var values = new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", _config["GoogleCalendarApi:ClientId"] },
            { "client_secret", _config["GoogleCalendarApi:ClientSecret"] },
            { "redirect_uri", _config["GoogleCalendarApi:RedirectUri"] },
            { "grant_type", "authorization_code" }
        };

        var content = new FormUrlEncodedContent(values);
        var response = await _http.PostAsync(tokenUrl, content);
        
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, new RetornoDTO { Sucesso = false, Mensagem = "Falha na troca de código por tokens.", Dados = json });
        }
        
        // 3. Desserialização e Extração dos Tokens
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        // Extrai Refresh Token (o que precisa ser salvo)
        var refreshTokenElement = root.TryGetProperty("refresh_token", out var rt) ? rt : default;
        string refreshToken = refreshTokenElement.ValueKind != JsonValueKind.Undefined ? rt.GetString() : null;
        
        // Extrai Access Token (para retorno imediato, se houver)
        var accessTokenElement = root.TryGetProperty("access_token", out var at) ? at : default;
        string accessToken = accessTokenElement.ValueKind != JsonValueKind.Undefined ? at.GetString() : null;


        // 4. Lógica de Persistência
        if (string.IsNullOrEmpty(refreshToken))
        {
             // Caso seja renovação (Refresh Token não é enviado novamente)
            return Ok(new RetornoDTO { Sucesso = true, Mensagem = "Token de acesso renovado. Refresh Token persistente permanece no DB." });
        }

        // 5. Salva (ou atualiza) o Refresh Token no PostgreSQL
        await _tokenRepositorio.SalvarRefreshTokenGoogleCalendarAsync(usuarioId, refreshToken); 

        // 6. Retorno de Sucesso
        return Ok(new RetornoDTO { 
            Sucesso = true, 
            Mensagem = "Autorização concluída e token persistente salvo.", 
            Dados = new { AccessToken = accessToken } 
        });
    }
}


// Classe auxiliar para receber o access token enviado pelo usuário
public class TokenData
{
    public string AccessToken { get; set; }
}