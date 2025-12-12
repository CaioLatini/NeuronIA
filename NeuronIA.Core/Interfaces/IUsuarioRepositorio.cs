using NeuronIA.Core.Modelos;
using NeuronIA.Core.DTOs;

namespace NeuronIA.Core.Interfaces;

public interface IUsuarioRepositorio
{
    Task CriarUsuarioAsync(UsuarioCriacaoDTO usuario);
    Task<UsuarioRespostaDTO?> PegarUsuarioPorIdAsync(int usuarioId);
    Task AtualizarUsuarioAsync(UsuarioAtualizacaoDTO usuario);
    Task DeletarUsuarioAsync(int usuarioId);
    //Task<UsuarioRespostaDTO?> PegarUsuarioPorEmailAsync(string email);
}