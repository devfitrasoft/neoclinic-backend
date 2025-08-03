using neo.admin.Models;
using Shared.Entities.Objs.Enterprise;

namespace neo.admin.Services
{
    public interface ICookieService
    {
        void SetAuth(HttpResponse response, TokenResultModel tokens, Login login);
        void ClearAuth(HttpResponse response);
    }
    public class CookieService : ICookieService
    {
        public void SetAuth(HttpResponse response, TokenResultModel tokens, Login login)
        {
            response.Cookies.Append("refresh_token", tokens.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            response.Cookies.Append("access_token", tokens.AccessToken, new CookieOptions
            {
                HttpOnly = false, // frontend JS can use this
                Secure = true,
                SameSite = SameSiteMode.None
            });

            response.Cookies.Append("faskes_name", login.Faskes.Name, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None
            });
        }

        public void ClearAuth(HttpResponse response)
        {
            response.Cookies.Delete("refresh_token");
            response.Cookies.Delete("access_token");
            response.Cookies.Delete("faskes_name");
        }
    }
}
