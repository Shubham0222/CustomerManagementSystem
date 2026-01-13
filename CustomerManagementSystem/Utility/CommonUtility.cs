using CustomerManagementSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Claims;
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
        public static bool Verify(string password, string? storedPassword)
        {
            var parts = storedPassword?.Split(':') ?? [];
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

        public static async Task SignInAsync(HttpContext httpContext,UserDto user,bool isPersistent = false)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.UserID.ToString())
                
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = isPersistent }
            );
        }

    }
}
