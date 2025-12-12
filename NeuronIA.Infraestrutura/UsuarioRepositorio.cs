using NeuronIA.Core.Interfaces;
using NeuronIA.Core.DTOs;
using NeuronIA.Core.Modelos;
using NeuronIA.Infraestrutura.Dados;
using NeuronIA.Infraestrutura.Services;

namespace NeuronIA.Infraestrutura;

public class UsuarioRepositorio : IUsuarioRepositorio
{
    private readonly NeuronIADbContext _dbContext;
    private readonly SenhaService _senhaService;
    public UsuarioRepositorio(NeuronIADbContext dbContext)
    {
        _dbContext = dbContext;
        _senhaService = new SenhaService();
    }

    public async Task CriarUsuarioAsync(UsuarioCriacaoDTO usuario)
    {
        if(!_senhaService.ValidarSenhaForte(usuario.Senha))
        {
            throw new Exception("Senha não atende aos requisitos de segurança.");
        }
        var usuarioModel = new ModeloUsuario
        {
            Nome = usuario.Nome,
            Email = usuario.Email,
            SenhaHash = _senhaService.HashSenha(usuario.Senha)
        };
        _dbContext.Add(usuarioModel);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<UsuarioRespostaDTO> PegarUsuarioPorIdAsync(int usuarioId)
    {
        var usuario = await _dbContext.Usuarios.FindAsync(usuarioId);
        if(usuario == null)
        {
            throw new Exception("Usuário não encontrado");
        }    
        var usuarioDTO = new UsuarioRespostaDTO
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email
        };
        return usuarioDTO;
    }
    public async Task AtualizarUsuarioAsync(UsuarioAtualizacaoDTO usuario)
    {
        var usuarioExistente = await _dbContext.Usuarios.FindAsync(usuario.Id);
        if(usuarioExistente == null)
        {
            throw new Exception("Usuário não encontrado");
        }
        
        //validar se a senha foi alterada, se sim, validar a força e hashear
        if(!string.IsNullOrEmpty(usuario.Senha))
        {
            if(!_senhaService.ValidarSenhaForte(usuario.Senha))
            {
                throw new Exception("Senha não atende aos requisitos de segurança.");
            }
            usuario.Senha = _senhaService.HashSenha(usuario.Senha);
        }

        usuarioExistente.Nome = usuario.Nome ?? usuarioExistente.Nome;
        usuarioExistente.Email = usuario.Email ?? usuarioExistente.Email;
        usuarioExistente.SenhaHash = usuario.Senha ?? usuarioExistente.SenhaHash;

        await  _dbContext.SaveChangesAsync();
    }
    public async Task DeletarUsuarioAsync(int usuarioId)
    {
        var usuario = await _dbContext.Usuarios.FindAsync(usuarioId);
        if(usuario != null)
        {
            _dbContext.Usuarios.Remove(usuario);
            await _dbContext.SaveChangesAsync();
        } else throw new Exception("Usuário não encontrado");
    } 
}
