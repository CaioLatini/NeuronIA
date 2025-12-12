using NeuronIA.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using NeuronIA.Core.DTOs;


[ApiController]
[Route("api/[controller]")]

public class UserController : ControllerBase
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    public UserController(IUsuarioRepositorio usuarioRepositorio)
    {
        _usuarioRepositorio = usuarioRepositorio;
    }

    [HttpPost("CrateUser")]
    public async Task<IActionResult> CriarUsuario([FromBody] UsuarioCriacaoDTO usuario)
    {
        await _usuarioRepositorio.CriarUsuarioAsync(usuario);
        return Ok();
    }

    [HttpGet("GetUserById")]
    public async Task<IActionResult> PegarUsuarioPorId([FromQuery] int usuarioId)
    {
        var usuario = await _usuarioRepositorio.PegarUsuarioPorIdAsync(usuarioId);
        return Ok(usuario);
    }
    [HttpPut("UpdateUser")]
    public async Task<IActionResult> AtualizarUsuario([FromBody] UsuarioAtualizacaoDTO usuario)
    {
        await _usuarioRepositorio.AtualizarUsuarioAsync(usuario);
        return Ok();
    }
    [HttpDelete("DeleteUser")]
    public async Task<IActionResult> DeletarUsuario([FromQuery] int usuarioId)
    {
        await _usuarioRepositorio.DeletarUsuarioAsync(usuarioId);
        return Ok();
    }
}