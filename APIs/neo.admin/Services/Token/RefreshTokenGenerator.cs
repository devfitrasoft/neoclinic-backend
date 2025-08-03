using System.Security.Cryptography;

namespace neo.admin.Services.Factories
{
    public interface IRefreshTokenGenerator
    {
        string GenerateToken();
    }

    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        public string GenerateToken()
        {
            var rng = RandomNumberGenerator.Create();
            var bytes = new byte[64];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes); // Store hash in DB as before
        }
    }
}
