using Shared.Common;
using Shared.Entities.Queries.Enterprise;
using Shared.Models;

namespace neo.admin.Facades
{
    public interface IResetPasswordFacade
    {
        Task<CommonAPIBodyResponseModel> IsOtpValidAsync(string otp, CancellationToken ct);
        Task<int> MarkIsUsedAsync(string otp, CancellationToken ct);
        Task<CommonAPIBodyResponseModel> UpdatePasswordAsync(string otp, string hashedPassword, CancellationToken ct);
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
