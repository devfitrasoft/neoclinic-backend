using neo.preregist.Common;
using neo.preregist.Models;
using neo.preregist.Services;
using Shared.Common;
using Shared.Entities.Queries;
using Shared.Entities.Queries.Enterprise;
using Shared.Models;

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
        private readonly OtpTokenQueries _otpQueries;
        private readonly PreRegistQueries _prQueries;
        private readonly ILogger<PreRegistFacades> _logger;

        public PreRegistFacades(ILogger<PreRegistFacades> logger, IConfiguration cfg, IOtpTokenDbContext otpCtx, 
            IPreRegistDbContext preRegCtx, MailService mail, ILoggerFactory loggerFactory)
        {
            _cfg = cfg;
            _mail = mail;
            _logger = logger;

            _otpQueries = new OtpTokenQueries(otpCtx);
            _prQueries = new PreRegistQueries(loggerFactory, preRegCtx);
        }

        public async Task<PreRegistResponse> SaveAndNotify(PreRegistRequest req, CancellationToken ct)
        {
            PreRegistResponse response = new PreRegistResponse();
            Tuple<string,DateTime> OtpAndExpiry = Tuple.Create(string.Empty, DateTime.UtcNow);

            try
            {

                //  1. Check whether existing row exist or not based on the type of comms being passed by users
                var row = await _prQueries.GetRowByMailsync(req, ct);

                if (row != null)
                {
                    //  2. Generate OTP & send mail only if this owner has yet to be registered system
                    if (row.IsRegistered)
                    {
                        response = GenerateResponse(PreRegistSaveResponse.Registered, false, "Registered", row.IsRegistered);
                    }
                    else
                    {
                        await _prQueries.UpdateInfoAsync(row, req, ct);
                        var otpRows = await _otpQueries.GetListOfNotYetUsedByTargetId(row.Id, OtpType.PreRegist, ct);

                        if (otpRows == null) // Meaning last OTP already been used, generate new row
                        {
                            OtpAndExpiry = Utilities.DoGenerateHashedOtp(_cfg["OtpToken :Expiry"]);
                            await _otpQueries.AddAsync(row.Id, OtpAndExpiry.Item1, OtpAndExpiry.Item2, OtpType.PreRegist, ct);

                            response = GenerateResponse(PreRegistSaveResponse.Updated, true, "Updated", row.IsRegistered);
                            return response;
                        }

                        foreach (var otpRow in otpRows) // Meaning old OTP has yet to be used, recycle
                        {
                            OtpAndExpiry = Utilities.DoGenerateHashedOtp(_cfg["OtpToken:Expiry"]);
                            await _otpQueries.RenewOtpAsync(otpRow, OtpAndExpiry.Item1, OtpAndExpiry.Item2, ct);
                        }

                        await _mail.SendNotifAsync(req.Email, OtpAndExpiry.Item1);

                        response = GenerateResponse(PreRegistSaveResponse.Updated, true, "Updated", row.IsRegistered);
                    }
                    return response;
                }

                //  2. Generate Otp character varying(255)
                OtpAndExpiry = Utilities.DoGenerateHashedOtp(_cfg["OtpToken:Expiry"]);

                //  3. Store the Pre-registration data along with Otp
                var newPreRegist = await _prQueries.AddAsync(req, ct);
                await _otpQueries.AddAsync(newPreRegist.Id, OtpAndExpiry.Item1, OtpAndExpiry.Item2, OtpType.PreRegist, ct);

                //  4. Send Mail/Whatsapp to the user
                await _mail.SendNotifAsync(req.Email, OtpAndExpiry.Item1);  //  later would add the whatsapp here
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General exception");
                response = GenerateResponse(PreRegistSaveResponse.Error, false, "Internal Error", false);
                return response;
            }

            response = GenerateResponse(PreRegistSaveResponse.Created, true, "Created", false);

            //  5. Send return response to caller so UI can show the 
            return response;
        }

        public async Task<PreRegistData?> GetRowByTokenAsync(string token, CancellationToken ct)
        {
            var preRegistAndExpiry = await _otpQueries.GetPreRegistAndExpiryByTokenAsync(token, ct);

            if (preRegistAndExpiry == null)
                return null;

            var preRegist = preRegistAndExpiry.Item1;

            return new PreRegistData(
                preRegist.Name,
                preRegist.Email,
                preRegist.Phone,
                preRegistAndExpiry.Item2,
                preRegist.IsRegistered
            );
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
                Data = new List<PreRegistResponseData>() { data }
            };

            return response;
        }
    }
}
