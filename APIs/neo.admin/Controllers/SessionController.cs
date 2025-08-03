using Microsoft.AspNetCore.Mvc;
using neo.admin.Data.Enterprise;
using neo.admin.Facades;
using neo.admin.Models;
using neo.admin.Services;
using neo.admin.Services.Token;
using Shared.Common;
using Shared.Models;
using System.ComponentModel.DataAnnotations;
using System.Security;

namespace neo.admin.Controllers
{
    [ApiController, Route("api/v1/session")]
    public class SessionController : ControllerBase
    {
        private readonly ILoginFacade _facade;
        private readonly ICookieService _cookieService;
        private readonly IHeaderService _headerService;

        private const string X_CLIENT_TYPE_WEB = "web";
        private const string X_CLIENT_TYPE_MOBILE = "mobile";

        public SessionController(ILogger<LoginFacade> facadeLogger, ITokenService tokenService, EnterpriseDbContext edb)
        {
            _facade = new LoginFacade(facadeLogger, tokenService, edb);
            _cookieService = new CookieService();
            _headerService = new HeaderService();
        }

        [HttpPost, Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestModelBase req, CancellationToken ct)
        {
            var clientType = Request.Headers["X-Client-Type"].FirstOrDefault()?.ToLower(); // "web" or "mobile"
            var deviceId = Request.Headers["X-Device-Id"].FirstOrDefault();
            var userAgent = Request.Headers["User-Agent"].ToString();

            try
            {
                if (IsClientContextInvalid(clientType, deviceId, userAgent))
                    return BadRequest(new LoginResponseModel()
                    {
                        Success = false,
                        Message = "Unknown device/client"
                    });


                var validationResults = new List<ValidationResult>();
                var context = new ValidationContext(req);

                if (!Validator.TryValidateObject(req, context, validationResults, validateAllProperties: true))
                    return BadRequest(new LoginResponseModel()
                    {
                        Success = false,
                        Message = StringParser.ValidationErrorMessageBuilder(validationResults)
                    });

                var result = await _facade.CreateSessionAsync(req, deviceId, userAgent, ct);

                if (result == null)
                    return StatusCode(StatusCodes.Status401Unauthorized, new CommonAPIBodyResponseModel()
                    {
                        Success = false,
                        Message = "Account did not exist"
                    });

                if (!result.Success)
                    return StatusCode(result.Message == "Invalid password"
                        ? StatusCodes.Status401Unauthorized
                        : StatusCodes.Status500InternalServerError, result);

                if (clientType == X_CLIENT_TYPE_WEB)
                    _cookieService.SetAuth(Response, result.Data.Tokens, result.Data.Login);

                if (clientType == X_CLIENT_TYPE_MOBILE)
                    _headerService.SetAuth(Response, result.Data.Tokens, result.Data.Login);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new CommonAPIBodyResponseModel
                {
                    Success = false,
                    Message = "Exception"
                });
            }

            return Ok(new CommonAPIBodyResponseModel
            {
                Success = true,
                Message = "Login successful"
            });
        }

        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync(CancellationToken ct)
        {
            var clientType = Request.Headers["X-Client-Type"].FirstOrDefault()?.ToLower(); // "web" or "mobile"
            var deviceId = Request.Headers["X-Device-Id"].FirstOrDefault();
            var userAgent = Request.Headers["User-Agent"].ToString();

            if (IsClientContextInvalid(clientType, deviceId, userAgent))
                return BadRequest(new LoginResponseModel()
                {
                    Success = false,
                    Message = "Unknown device/client"
                });


            var refreshToken = clientType == X_CLIENT_TYPE_WEB
                ? Request.Cookies["refresh_token"]
                : Request.Headers["X-Refresh-Token"].FirstOrDefault();

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new LoginResponseModel()
                {
                    Success = false,
                    Message = "Missing token"
                });

            try
            {
                var result = await _facade.RotateRefreshTokenAsync(refreshToken, deviceId, userAgent, ct);

                if (clientType == X_CLIENT_TYPE_WEB)
                    _cookieService.SetAuth(Response, result.Tokens, result.Login);

                if (clientType == X_CLIENT_TYPE_MOBILE)
                    _headerService.SetAuth(Response, result.Tokens, result.Login);

                return Ok(new { success = true, message = "Token refreshed" });
            }
            catch (SecurityException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("logout")]
        public async Task<IActionResult> LogoutAsync(CancellationToken ct)
        {
            var clientType = Request.Headers["X-Client-Type"].FirstOrDefault()?.ToLower(); // "web" or "mobile"
            var deviceId = Request.Headers["X-Device-Id"].FirstOrDefault();
            var userAgent = Request.Headers["User-Agent"].ToString();

            if (IsClientContextInvalid(clientType, deviceId, userAgent))
                return BadRequest(new LoginResponseModel()
                {
                    Success = false,
                    Message = "Unknown device/client"
                });

            var refreshToken = clientType == X_CLIENT_TYPE_WEB
                ? Request.Cookies["refresh_token"]
                : Request.Headers["X-Refresh-Token"].FirstOrDefault();

            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new { success = false, message = "Missing token" });

            var session = await _facade.GetSessionByRefreshTokenAsync(refreshToken, deviceId, userAgent, ct);
            if (session == null)
                return Unauthorized(new { success = false, message = "Invalid session or token" });

            await _facade.LogoutAsync(session.LoginId, deviceId, userAgent, ct);


            if (clientType == X_CLIENT_TYPE_WEB)
                _cookieService.ClearAuth(Response);

            if (clientType == X_CLIENT_TYPE_MOBILE)
                _headerService.ClearAuth(Response);

            return Ok(new { success = true, message = "Logged out successfully" });
        }

        private bool IsClientContextInvalid(string? clientType, string? deviceId, string? userAgent) =>
            string.IsNullOrWhiteSpace(clientType) ||
            string.IsNullOrWhiteSpace(deviceId) ||
            string.IsNullOrWhiteSpace(userAgent);
    }
}
