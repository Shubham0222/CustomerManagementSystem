using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace CustomerManagementSystem.Utility
{
    public static class CommonUtility
    {

        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100000;

        // HASH PASSWORD (store salt inside hash)
        public static string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            byte[] hash = KeyDerivation.Pbkdf2(
                password,
                salt,
                KeyDerivationPrf.HMACSHA256,
                Iterations,
                KeySize
            );

            // Store as: salt:hash
            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        // VERIFY PASSWORD (extract salt automatically)
        public static bool Verify(string password, string storedPassword)
        {
            var parts = storedPassword.Split(':');
            if (parts.Length != 2)
                return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHash = Convert.FromBase64String(parts[1]);

            byte[] computedHash = KeyDerivation.Pbkdf2(
                password,
                salt,
                KeyDerivationPrf.HMACSHA256,
                Iterations,
                KeySize
            );

            return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
        }
    }
}
