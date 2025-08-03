using neo.admin.Models;
using Shared.Entities.Objs.Enterprise;

namespace neo.admin.Services
{
    public interface IHeaderService
    {
        void SetAuth(HttpResponse response, TokenResultModel tokens, Login login);
        void ClearAuth(HttpResponse response);
    }
    public class HeaderService : IHeaderService
    {
        public void SetAuth(HttpResponse response, TokenResultModel tokens, Login login)
        {
            response.Headers["X-Access-Token"] = tokens.AccessToken;
            response.Headers["X-Refresh-Token"] = tokens.RefreshToken;
            response.Headers["X-Faskes-Name"] = login.Faskes?.Name ?? string.Empty;

            var accessExpiresInSec = (int)(tokens.AccessTokenExpiry - DateTime.UtcNow).TotalSeconds;
            var refreshExpiresInSec = (int)(tokens.RefreshTokenExpiry - DateTime.UtcNow).TotalSeconds;

            response.Headers["X-Token-Expires-In"] = accessExpiresInSec.ToString();
            response.Headers["X-Refresh-Expires-In"] = refreshExpiresInSec.ToString();
        }

        public void ClearAuth(HttpResponse response)
        {
            response.Headers.Remove("X-Access-Token");
            response.Headers.Remove("X-Refresh-Token");
            response.Headers.Remove("X-Faskes-Name");
            response.Headers.Remove("X-Token-Expires-In");
            response.Headers.Remove("X-Refresh-Expires-In");
        }
    }
}
