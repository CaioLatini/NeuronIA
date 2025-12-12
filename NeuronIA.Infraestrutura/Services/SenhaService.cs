using System.Text.RegularExpressions;

namespace NeuronIA.Infraestrutura.Services
{
    public class SenhaService
    {
        public bool ValidarSenhaForte(string senha)
        {
            // Critérios de senha forte:
            // Pelo menos 8 caracteres
            // Pelo menos uma letra maiúscula
            // Pelo menos uma letra minúscula
            // Pelo menos um dígito
            // Pelo menos um caractere especial

            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
            return regex.IsMatch(senha);
        }
        public string HashSenha(string senha)
        {
            // Implementação simples de hash (não é muito seguro - para produção é bom mudar)
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(senha);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
        public async Task<bool> VerificarSenha(string senha, string hash)
        {
            var hashSenha = HashSenha(senha);
            return hashSenha == hash;
        }
    }
}