namespace User.Api.Services.Security.Cryptography
{
    public interface IBcryptCryptography
    {
        public string GenerateCryptography(string key);
        public bool IsKeyValid(string key, string hashKey);
    }
}
