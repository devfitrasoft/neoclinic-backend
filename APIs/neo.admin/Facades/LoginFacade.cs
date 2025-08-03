using neo.admin.Data.Enterprise;
using neo.admin.Models;
using neo.admin.Services.Token;
using Shared.Entities.Objs.Enterprise;
using Shared.Entities.Queries.Enterprise;
using System.Security;

namespace neo.admin.Facades
{
    public interface ILoginFacade
    {
        Task<LoginResponseModel> CreateSessionAsync(LoginRequestModelBase req, string deviceId, string userAgent, CancellationToken ct);
        Task<AuthSession?> GetSessionByRefreshTokenAsync(string hashedRefreshToken, string deviceId, string userAgent, CancellationToken ct);
        Task<SessionDataModel> RotateRefreshTokenAsync(string hashedRefreshToken, string deviceId, string userAgent, CancellationToken ct);
        Task<int> LogoutAsync(long loginId, string deviceId, string userAgent, CancellationToken ct);
        Task LogoutAllAsync(CancellationToken ct);
    }

    public sealed class LoginFacade : ILoginFacade
    {
        private ILogger<LoginFacade> _logger;

        private readonly LoginQueries _loginQry;
        private readonly AuthSessionQueries _sessionQry;

        private readonly ITokenService _tokenService;
        
        public LoginFacade(ILogger<LoginFacade> logger, ITokenService tokenService, EnterpriseDbContext edb) 
        {
            _logger = logger;
            _tokenService = tokenService;

            _loginQry = new LoginQueries(edb);
            _sessionQry = new AuthSessionQueries(edb);
        }

        public async Task<LoginResponseModel> CreateSessionAsync(LoginRequestModelBase req, string deviceId, string userAgent, CancellationToken ct)
        {
            var result = new LoginResponseModel();
            try
            {
                var login = await _loginQry.GetActiveByUsernameAsync(req.Username, ct);

                if (login == null || !BCrypt.Net.BCrypt.Verify(req.Password, login.PasswordHash))
                    return new LoginResponseModel { Success = false, Message = "Invalid password" };

                var tokens = await _tokenService.GenerateTokensAsync(login);
                var resSessionStore = await _sessionQry.AddSessionAsync(login.Id, tokens.RefreshToken, deviceId, userAgent, ct);

                result = new LoginResponseModel() //return all in cookie instead of body response
                {
                    Success = true,
                    Data = new SessionDataModel
                    {
                        Tokens = tokens,
                        Login = login
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                result = new LoginResponseModel()
                {
                    Success = false,
                    Message = "Exception",
                };
            }

            return result;
        }

        public async Task<AuthSession?> GetSessionByRefreshTokenAsync(string hashedRefreshToken, string deviceId, string userAgent, CancellationToken ct)
            => await _sessionQry.GetSessionRefreshTokenAsync(hashedRefreshToken, deviceId, userAgent, ct);

        public async Task<SessionDataModel> RotateRefreshTokenAsync(string hashedRefreshToken, string deviceId, string userAgent, CancellationToken ct)
        {
            var session = await _sessionQry.GetSessionRefreshTokenAsync(hashedRefreshToken, deviceId, userAgent, ct);

            if (session == null)
                throw new SecurityException("Invalid or expired refresh token");

            var idleTimeout = TimeSpan.FromMinutes(60);
            if (DateTime.UtcNow - session.LastActiveAt > idleTimeout)
            {
                await _sessionQry.InvalidateAllSessionsByLoginIdPerDevice(session.LoginId, deviceId, userAgent, ct);
                throw new SecurityException("Session timed out");
            }

            var login = await _loginQry.GetById(session.LoginId, ct);

            if(login == null)
                throw new SecurityException("Invalid credential");


            var invalidateSessions = await _sessionQry.InvalidateAllSessionsByLoginIdPerDevice(session.LoginId, deviceId, userAgent, ct);

            var newToken = await _tokenService.GenerateTokensAsync(login);

            var refreshSessionAsync = await _sessionQry.AddSessionAsync(session.LoginId, newToken.RefreshToken, deviceId, userAgent, ct);

            if (refreshSessionAsync == null)
                throw new SecurityException("Invalid or expired refresh token");

            if (invalidateSessions <= 0)
                throw new SecurityException("Invalid credential");

            return new SessionDataModel()
            {
                Login = login,
                Tokens = newToken
            };
        }

        public async Task<int> LogoutAsync(long loginId, string deviceId, string userAgent, CancellationToken ct)
            => await _sessionQry.InvalidateAllSessionsByLoginIdPerDevice(loginId, deviceId, userAgent, ct);

        public async Task LogoutAllAsync(CancellationToken ct)
        {
            await _sessionQry.InvalidateAllSessionsAsync(ct);
            await _sessionQry.ResetAuthSessionsAsync(ct);
        }
    }
}
