using Microsoft.AspNetCore.Mvc;
using neo.admin.Data.Enterprise;
using neo.admin.Facades;
using neo.admin.Models;
using neo.admin.Services;
using Shared.Common;
using Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace neo.admin.ApiControllers
{
    [ApiController, Route("api/v1/password")]
    public class PasswordApiController : ControllerBase
    {
        private readonly IResetPasswordFacade _resetFacade;
        public PasswordApiController(ILoggerFactory loggerFactory, IConfiguration cfg, EnterpriseDbContext edb, MailService mailService)
            => _resetFacade = new ResetPasswordFacade(loggerFactory, cfg, edb, mailService);

        [HttpGet, Route("send-request")]
        public async Task<IActionResult> SendRequestAsync([FromQuery] string username, CancellationToken ct)
        {
            try
            {
                // Do not pass request-bound CancellationToken!
                await _resetFacade.GenerateAndSendRequestOtpAsync(username, ct);
            }
            catch (Exception ex)
            {
                // Optional: log globally if needed
                Console.WriteLine("Unhandled error in background OTP sender: " + ex.Message);
            }

            return Ok(new CommonAPIBodyResponseModel() { Success = true });
        }

        [HttpPost, Route("reset")]
        public async Task<IActionResult> ResetAsync([FromQuery] string otp, [FromBody] ResetPasswordReq req, CancellationToken ct)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(req);

            if (!Validator.TryValidateObject(req, context, validationResults, validateAllProperties: true))
                return StatusCode(StatusCodes.Status400BadRequest, new CommonAPIBodyResponseModel()
                {
                    Success = false,
                    Message = StringParser.ValidationErrorMessageBuilder(validationResults)
                });

            var otpIsValid = await _resetFacade.IsOtpValidAsync(otp, ct);

            if (!otpIsValid.Success)
                return StatusCode(StatusCodes.Status400BadRequest, otpIsValid); // Prompt user to ask for new OTP

            int resOtpUsage = await _resetFacade.MarkIsUsedAsync(otp, ct);  // Expire the current OTP

            if (resOtpUsage == 0)
                return StatusCode(StatusCodes.Status400BadRequest, new CommonAPIBodyResponseModel()
                {
                    Success = false,
                    Message = "Invalid OTP"
                });

            if (resOtpUsage == 2)
                return StatusCode(StatusCodes.Status400BadRequest, new CommonAPIBodyResponseModel()
                {
                    Success = false,
                    Message = "OTP has been used"
                });

            // Validate password match
            if (req.Password != req.RePassword)
                return StatusCode(StatusCodes.Status400BadRequest, new CommonAPIBodyResponseModel()
                {
                    Success = false,
                    Message = "Passwords do not match."
                });

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(req.Password);

            var result = await _resetFacade.UpdatePasswordAsync(otp, hashedPassword, ct);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, result);
            }
        }
    }
}
