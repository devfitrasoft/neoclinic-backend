using neo.preregist.Common;
using neo.preregist.Data.Enterprise;
using neo.preregist.Models;
using neo.preregist.Queries;
using neo.preregist.Services;
using System.Security.Cryptography;
using System.Text;

namespace neo.preregist.Facades
{
    public interface IPreRegistFacade 
    {
        Task<PreRegistResponse> SaveAndNotify(PreRegistRequest req, CancellationToken ct);
        Task<PreRegistData?> GetRowByTokenAsync(string token, CancellationToken ct);
    }

    public sealed class PreRegistFacades : IPreRegistFacade
    {

        private readonly MailService _mail;
        private readonly IConfiguration _cfg;
        private readonly PreRegistQueries _query;
        private readonly ILogger<PreRegistFacades> _logger;
        public PreRegistFacades(ILogger<PreRegistFacades> logger, IConfiguration cfg, EnterpriseDbContext edb, MailService mail)
        {
            _cfg = cfg;
            _mail = mail;
            _logger = logger;

            _query = new PreRegistQueries(edb);
        }

        public async Task<PreRegistResponse> SaveAndNotify(PreRegistRequest req, CancellationToken ct)
        {
            PreRegistResponse response = new PreRegistResponse();
            Tuple<string,DateTime> OtpAndExpiry = Tuple.Create(string.Empty, DateTime.UtcNow);

            try
            {

                //  1. Check whether existing row exist or not based on the type of comms being passed by users
                var row = await _query.GetRowByMailsync(req, ct);

                if (row != null)
                {
                    //  2. Generate OTP & send mail only if this owner has yet to be registered system
                    if (row.IsRegistered)
                    {
                        response = GenerateResponse(PreRegistSaveResponse.Registered, false, "Registered", row.IsRegistered);
                    }
                    else
                    {
                        OtpAndExpiry = DoGenerateHashedOtp();
                        await _query.UpdateInfoAndOtpAsync(row, OtpAndExpiry.Item1, OtpAndExpiry.Item2, req, ct);
                        await _mail.SendNotifAsync(req.Email, OtpAndExpiry.Item1);

                        response = GenerateResponse(PreRegistSaveResponse.Updated, true, "Updated", row.IsRegistered);
                    }
                    return response;
                }

                //  2. Generate Otp character varying(255)
                OtpAndExpiry = DoGenerateHashedOtp();

                //  3. Store the Pre-registration data along with Otp
                await _query.AddAsync(req, OtpAndExpiry.Item1, OtpAndExpiry.Item2, ct);

                //  4. Send Mail/Whatsapp to the user
                await _mail.SendNotifAsync(req.Email, OtpAndExpiry.Item1);  //  later would add the whatsapp here
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General exception");
                response = GenerateResponse(PreRegistSaveResponse.Error, false, "Internal Error", false);
            }

            response = GenerateResponse(PreRegistSaveResponse.Created, true, "Created", false);

            //  6. Send return response to caller so UI can show the 
            return response;
        }

        public async Task<PreRegistData?> GetRowByTokenAsync(string token, CancellationToken ct)
        {
            var row = await _query.GetRowByTokenAsync(token, ct);

            if(row == null)
            {
                return null;
            }

            return new PreRegistData(
                row.Name, 
                row.Email,
                row.Phone,
                row.OtpExpiresAt,
                row.IsRegistered
            );
        }
            
        private Tuple<string,DateTime> DoGenerateHashedOtp()
        {
            string plainOtp = GenerateOtp();
            string hashedOtp = HashOtp(plainOtp);
            var otpExpiryMinutes = int.Parse(_cfg["PreRegistToken:Expiry"] ?? LocalConstants.OTP_EXPIRY_IN_MINUTE);
            var expiresAt = DateTime.UtcNow.AddMinutes(otpExpiryMinutes);

            return Tuple.Create(hashedOtp, expiresAt);
        }

        private string GenerateOtp()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[6];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes).TrimEnd('=');
        }

        private string HashOtp(string otp)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(otp);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private PreRegistResponse GenerateResponse(PreRegistSaveResponse status, bool isSuccess = false, string message = "", bool isRegistered = false)
        {
            var data = new PreRegistResponseData(
                status,
                isRegistered
            );
            
            var response = new PreRegistResponse()
            {
                Success = isSuccess,
                Message = message,
                Data = data
            };

            return response;
        }
    }
}
