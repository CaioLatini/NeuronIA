using Microsoft.EntityFrameworkCore;
using NeuronIA.Infraestrutura;
using NeuronIA.Infraestrutura.Dados;
using NeuronIA.Core.Interfaces;
using NeuronIA.Infraestrutura.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------
// CONFIGURAÇÃO DO BANCO
// ---------------------------------------------
var stringConexao = builder.Configuration.GetConnectionString("ConexaoPadrao");

builder.Services.AddDbContext<NeuronIADbContext>(options =>
    options.UseNpgsql(stringConexao));

// ---------------------------------------------
// INJEÇÃO DE DEPENDÊNCIA
// ---------------------------------------------
builder.Services.AddScoped<ITokenRepositorioGoogleCalendar, TokenRepositorioGoogleCalendar>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<SenhaService>();
builder.Services.AddScoped<IGoogleTokenService, GoogleTokenService>();

// ---------------------------------------------
// CONTROLLERS + SWAGGER
// ---------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Para requisições externas
builder.Services.AddHttpClient();

var app = builder.Build();

// ---------------------------------------------
// SWAGGER NO DESENVOLVIMENTO
// ---------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ---------------------------------------------
// PIPELINE DO ASP.NET
// ---------------------------------------------
app.UseHttpsRedirection();

// IMPORTANTE → habilita o roteamento
app.UseRouting();

// caso tenha autenticação, ele deve vir ENTRE UseRouting e UseEndpoints
app.UseAuthorization();

// registra os controllers corretamente
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// ---------------------------------------------
app.Run();
