using Shared.Common;
using Shared.Entities.Queries.Enterprise;
using Shared.Models;

namespace neo.admin.Facades
{
    public interface IResetPasswordFacade
    {
        Task<CommonAPIBodyResponse> IsOtpValidAsync(string otp, CancellationToken ct);
        Task MarkIsUsedAsync(string otp, CancellationToken ct);
        Task<CommonAPIBodyResponse> UpdatePasswordAsync(string otp, string hashedPassword, CancellationToken ct);
    }

    public sealed class ResetPasswordFacade : IResetPasswordFacade
    {
        private readonly LoginQueries _loginQry;
        private readonly OtpTokenQueries _otpQry;

        public ResetPasswordFacade(LoginQueries loginQueries, OtpTokenQueries otpQry) 
        {
            _otpQry = otpQry;
            _loginQry = loginQueries;
        }

        public async Task<CommonAPIBodyResponse> IsOtpValidAsync(string otp, CancellationToken ct)
        {
            var isValidOtp = await _otpQry.IsOtpUnused(otp, OtpType.ResetPwd, ct);

            return new CommonAPIBodyResponse()
            {
                Success = isValidOtp,
                Message = !isValidOtp ? "Otp is not valid" : string.Empty,
            };
        }

        public async Task MarkIsUsedAsync(string otp, CancellationToken ct)
            => await _otpQry.MarkIsUsedAsync(otp, OtpType.ResetPwd, ct);

        public async Task<CommonAPIBodyResponse> UpdatePasswordAsync(string otp, string hashedPassword, CancellationToken ct)
        {
            var loginId = await _otpQry.GetTargetIdByOtpAsync(otp, OtpType.ResetPwd, ct);
            if (loginId == null)
            {
                return new CommonAPIBodyResponse() { Success = false, Message = "User not found." };
            }

            var result = await _loginQry.UpdatePasswordAsync(loginId.Value, hashedPassword, ct);

            return new CommonAPIBodyResponse() { Success = result > 0, Message = result > 0 ? "Password has been updated" : "Update password failed" };
        }
    }
}
