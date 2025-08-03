using neo.admin.Models;
using neo.admin.Services.Factories;
using Shared.Entities.Objs.Enterprise;

namespace neo.admin.Services.Token
{
    public interface ITokenService
    {
        Task<TokenResultModel> GenerateTokensAsync(Login login);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _cfg;
        private readonly IJwtProvider _jwtProvider;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;

        public TokenService(IConfiguration cfg, IJwtProvider jwtProvider, IRefreshTokenGenerator refreshTokenGenerator)
        {
            _cfg = cfg;
            _jwtProvider = jwtProvider;
            _refreshTokenGenerator = refreshTokenGenerator;
        }

        public async Task<TokenResultModel> GenerateTokensAsync(Login login)
        {
            var accessExpiryMinutes = _cfg.GetValue<int>("JwtToken:AccessExpiry");
            var refreshExpiryMinutes = _cfg.GetValue<int>("JwtToken:RefreshExpiry");

            var accessToken = _jwtProvider.GenerateToken(login); // stored in browser's memory/mobile's RAM
            var refreshToken = _refreshTokenGenerator.GenerateToken();  // stored in AuthSession
            var expiresIn = (int)TimeSpan.FromHours(_cfg.GetValue<int>("JwtToken:RefreshExpiry")).TotalSeconds;

            var accessExpiry = DateTime.UtcNow.AddMinutes(accessExpiryMinutes);
            var refreshExpiry = DateTime.UtcNow.AddMinutes(refreshExpiryMinutes);

            return await Task.FromResult(new TokenResultModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = accessExpiry,
                RefreshTokenExpiry = refreshExpiry,
                ExpiresIn = (int)TimeSpan.FromMinutes(accessExpiryMinutes).TotalSeconds
            });
        }
    }
}
