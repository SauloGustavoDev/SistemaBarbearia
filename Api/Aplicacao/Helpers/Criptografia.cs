using System.Security.Cryptography;

namespace Api.Aplicacao.Helpers
{
    public static class Criptografia
    {
        public static string GerarSenha(string password, int iterations = 100_000)
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[16];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32); // 256-bit

            return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        // Verifica: retorna true se coincidir
        public static bool VerificarSenha(string password, string stored)
        {
            // stored: iterations.salt.hash
            var parts = stored?.Split('.');
            if (parts == null || parts.Length != 3) return false;

            int iterations = int.Parse(parts[0]);
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] storedHash = Convert.FromBase64String(parts[2]);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            byte[] computed = pbkdf2.GetBytes(storedHash.Length);

            return CryptographicOperations.FixedTimeEquals(computed, storedHash);
        }
    }
}
