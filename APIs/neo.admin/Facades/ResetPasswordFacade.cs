using neo.admin.Data.Enterprise;
using neo.admin.Services;
using Shared.Common;
using Shared.Entities.Objs.Enterprise;
using Shared.Entities.Queries.Enterprise;
using Shared.Models;

namespace neo.admin.Facades
{
    public interface IResetPasswordFacade
    {
        Task<CommonAPIBodyResponseModel> IsOtpValidAsync(string otp, CancellationToken ct);
        Task<int> MarkIsUsedAsync(string otp, CancellationToken ct);
        Task<CommonAPIBodyResponseModel> UpdatePasswordAsync(string otp, string hashedPassword, CancellationToken ct);
        Task GenerateAndSendRequestOtpAsync(string mailtarget, CancellationToken ct);
    }

    public sealed class ResetPasswordFacade : IResetPasswordFacade
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _cfg;
        private readonly LoginQueries _loginQry;
        private readonly OtpTokenQueries _otpQry;
        private readonly MailService _mailService;

        public ResetPasswordFacade(ILoggerFactory loggerFactory, IConfiguration cfg, EnterpriseDbContext edb, MailService mailService)
        {
            _cfg = cfg;
            _logger = loggerFactory.CreateLogger<ResetPasswordFacade>();
            
            _loginQry = new LoginQueries(edb);
            _otpQry = new OtpTokenQueries(edb);

            _mailService = mailService;
        }

        public async Task GenerateAndSendRequestOtpAsync(string username, CancellationToken ct)
        {
            var login = await _loginQry.GetActiveByUsernameAsync(username, ct);

            if (login == null)
            {
                _logger.LogError("GenerateAndSendRequestOtp: Login not found");
            }

            var OtpAndExpiry = Utilities.DoGenerateHashedOtp(_cfg.GetValue<int>("JwtToken:RefreshExpiry"));
            int resOtp = await _otpQry.AddAsync(login.Id, OtpAndExpiry.Item1, OtpAndExpiry.Item2, OtpType.ResetPwd, ct);

            if (resOtp < 0)
            {
                _logger.LogError("GenerateAndSendRequestOtp: Unable to generate new OTP");
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    await _mailService.SendPassResetAsync(login.Email, OtpAndExpiry, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError("GenerateAndSendRequestOtp: Failed to send email");
                }
                });
        }

        public async Task<CommonAPIBodyResponseModel> IsOtpValidAsync(string otp, CancellationToken ct)
        {
            var isValidOtp = await _otpQry.IsOtpUnused(otp, OtpType.ResetPwd, ct);

            return new CommonAPIBodyResponseModel()
            {
                Success = isValidOtp,
                Message = !isValidOtp ? "Otp is not valid" : string.Empty,
            };
        }

        public async Task<int> MarkIsUsedAsync(string otp, CancellationToken ct)
            => await _otpQry.MarkIsUsedAsync(otp, OtpType.ResetPwd, ct);

        public async Task<CommonAPIBodyResponseModel> UpdatePasswordAsync(string otp, string hashedPassword, CancellationToken ct)
        {
            var loginId = await _otpQry.GetTargetIdByOtpAsync(otp, OtpType.ResetPwd, ct);
            if (loginId == null)
            {
                return new CommonAPIBodyResponseModel() { Success = false, Message = "User not found." };
            }

            var result = await _loginQry.UpdatePasswordAsync(loginId.Value, hashedPassword, ct);

            return new CommonAPIBodyResponseModel() { Success = result > 0, Message = result > 0 ? "Password has been updated" : "Update password failed" };
        }
    }
}
