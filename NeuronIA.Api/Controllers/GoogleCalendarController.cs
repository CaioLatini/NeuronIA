using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using NeuronIA.Core.DTOs;
using NeuronIA.Core.Interfaces;


[ApiController]
[Route("api/[controller]")]
public class GoogleCalendarController : ControllerBase
{
    private readonly IGoogleTokenService _tokenService;
    private readonly HttpClient _http;
    // O IConfiguration é necessário caso precisemos de IDs de calendário ou outras configurações

    public GoogleCalendarController(IGoogleTokenService tokenService, HttpClient http)
    {
        _tokenService = tokenService;
        _http = http;
    }

    // ----------------------------------------------------
    // 1. CRIAR EVENTO (POST)
    // ----------------------------------------------------
    [HttpPost("CreateEvent")]
    public async Task<IActionResult> CriarEvento([FromBody] CriarEventoCalendar request)
    {
        try
        {
            //Obtém um Access Token válido
            string accessToken = await _tokenService.ObterTokenAsync(request.UsuarioId);
            
            var apiUrl = "https://www.googleapis.com/calendar/v3/calendars/primary/events";
            
            // 2. Monta o corpo do evento no formato JSON (simplificado)
            var eventPayload = new
            {
                summary = request.Summary,
                location = request.Location,
                description = request.Description,
                start = new {
                    dateTime = request.StartTime.ToString("o"), // Formato ISO 8601
                    timeZone = request.TimeZone
                },
                end = new {
                    dateTime = request.EndTime.ToString("o"),
                    timeZone = request.TimeZone
                }
            };

            // 3. Faz a chamada à API do Google
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var jsonContent = new StringContent(JsonSerializer.Serialize(eventPayload), Encoding.UTF8, "application/json");

            var response = await _http.PostAsync(apiUrl, jsonContent);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new RetornoDTO { Sucesso = false, Mensagem = "Falha ao criar evento no Google Calendar.", Dados = responseJson });
            }

            return Ok(new RetornoDTO { Sucesso = true, Mensagem = "Evento criado com sucesso.", Dados = JsonSerializer.Deserialize<JsonElement>(responseJson) });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new RetornoDTO { Sucesso = false, Mensagem = ex.Message });
        }
    }


    // ----------------------------------------------------
    // 2. LISTAR EVENTOS (GET)
    // ----------------------------------------------------
    [HttpGet("ListEvents")]
    public async Task<IActionResult> ListarEventos([FromQuery] ListarEventosCalendar request)
    {
        try
        {
            string accessToken = await _tokenService.ObterTokenAsync(request.UsuarioId);

            // Converte as datas para o formato RFC3339, obrigatório na API do Google
            string timeMin = request.Inicio.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
            string timeMax = request.Fim.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
            
            var apiUrl = $"https://www.googleapis.com/calendar/v3/calendars/primary/events?timeMin={timeMin}&timeMax={timeMax}&singleEvents=true&orderBy=startTime";
            
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _http.GetAsync(apiUrl);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new RetornoDTO { Sucesso = false, Mensagem = "Falha ao listar eventos do Google Calendar.", Dados = responseJson });
            }

            return Ok(new RetornoDTO { Sucesso = true, Mensagem = "Eventos listados com sucesso.", Dados = JsonSerializer.Deserialize<JsonElement>(responseJson) });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new RetornoDTO { Sucesso = false, Mensagem = ex.Message });
        }
    }


    // ----------------------------------------------------
    // 3. ATUALIZAR EVENTO (PUT)
    // ----------------------------------------------------
    // O ID do evento (EventId) e o DTO de edição virão na URL e no corpo.
    [HttpPut("UpdateEvent/{eventId}")]
    public async Task<IActionResult> AtualizarEvento(string eventId, [FromBody] EditarEventoCalendar request)
    {
        try
        {
            string accessToken = await _tokenService.ObterTokenAsync(request.UsuarioId);
            
            // Endpoint para PATCH ou PUT (PATCH é mais eficiente)
            var apiUrl = $"https://www.googleapis.com/calendar/v3/calendars/primary/events/{eventId}";
            
            // Monta o corpo de atualização (apenas os campos do DTO)
            var eventPayload = new
            {
                summary = request.Summary,
                location = request.Location,
                description = request.Description,
                start = new {
                    dateTime = request.StartTime.ToString("o"),
                    timeZone = request.TimeZone
                },
                end = new {
                    dateTime = request.EndTime.ToString("o"),
                    timeZone = request.TimeZone
                }
            };

            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var jsonContent = new StringContent(JsonSerializer.Serialize(eventPayload), Encoding.UTF8, "application/json");
            
            // PUT para substituir o evento inteiro
            var response = await _http.PutAsync(apiUrl, jsonContent); 
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new RetornoDTO { Sucesso = false, Mensagem = "Falha ao atualizar evento no Google Calendar.", Dados = responseJson });
            }

            return Ok(new RetornoDTO { Sucesso = true, Mensagem = "Evento atualizado com sucesso.", Dados = JsonSerializer.Deserialize<JsonElement>(responseJson) });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new RetornoDTO { Sucesso = false, Mensagem = ex.Message });
        }
    }


    // ----------------------------------------------------
    // 4. APAGAR EVENTO (DELETE)
    // O parâmetro necessário é o ID do evento (EventId)
    // ----------------------------------------------------
    [HttpDelete("DeleteEvent/{usuarioId}/{eventId}")]
    public async Task<IActionResult> ApagarEvento(int usuarioId, string eventId)
    {
        try
        {
            string accessToken = await _tokenService.ObterTokenAsync(usuarioId);
            
            var apiUrl = $"https://www.googleapis.com/calendar/v3/calendars/primary/events/{eventId}";
            
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _http.DeleteAsync(apiUrl);
            
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return Ok(new RetornoDTO { Sucesso = true, Mensagem = $"Evento ID {eventId} apagado com sucesso." });
            }
            
            var responseJson = await response.Content.ReadAsStringAsync();

            return StatusCode((int)response.StatusCode, new RetornoDTO { Sucesso = false, Mensagem = "Falha ao apagar evento no Google Calendar.", Dados = responseJson });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new RetornoDTO { Sucesso = false, Mensagem = ex.Message });
        }
    }
}