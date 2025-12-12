namespace NeuronIA.Core.Interfaces
{
    public interface IGoogleTokenService
    {
        Task<string> ObterTokenAsync(int usuarioId);
    }
}