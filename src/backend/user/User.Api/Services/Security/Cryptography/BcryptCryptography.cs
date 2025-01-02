
using BCrypt.Net;

namespace User.Api.Services.Security.Cryptography
{
    public class BcryptCryptography : IBcryptCryptography
    {
        public string GenerateCryptography(string key)
        {
            return BCrypt.Net.BCrypt.HashPassword(key);
        }

        public bool IsKeyValid(string key, string hashKey)
        {
            return BCrypt.Net.BCrypt.Verify(key, hashKey);
        }
    }
}
