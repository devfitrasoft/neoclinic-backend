using neo.admin.Models;
using Shared.Entities.Objs.Enterprise;

namespace neo.admin.Services
{
    public interface IHeaderService
    {
        void SetAuth(HttpResponse response, TokenResultModel tokens, Login login, LoginSessionContext? context = null);
        void ClearAuth(HttpResponse response);
    }
    public class HeaderService : IHeaderService
    {
        public void SetAuth(HttpResponse response, TokenResultModel tokens, Login login, LoginSessionContext? context = null)
        {
            response.Headers["X-Access-Token"] = tokens.AccessToken;
            response.Headers["X-Refresh-Token"] = tokens.RefreshToken;
            response.Headers["X-Faskes-Name"] = login.Faskes?.Name ?? string.Empty;

            if (context?.TanggalJaga != null)
                response.Headers["X-Tanggal-Jaga"] = context.TanggalJaga.Value.ToString();

            if (context?.Shift != null)
                response.Headers["X-Shift"] = context.Shift.ToString();
        }

        public void ClearAuth(HttpResponse response)
        {
            response.Headers.Remove("X-Access-Token");
            response.Headers.Remove("X-Refresh-Token");
            response.Headers.Remove("X-Faskes-Name");
        }
    }
}
