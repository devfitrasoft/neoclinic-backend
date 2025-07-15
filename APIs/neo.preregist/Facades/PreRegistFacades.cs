using neo.admin.Common;
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
            PrefComms prefComm = (PrefComms)req.PrefComm;
            ProductTypes product = (ProductTypes)req.ProductType;
            try
            {

                //  1. Check whether existing row exist or not based on the type of comms being passed by users
                var row = await _query.GetRowByMailOrPhoneAsync(req, ct);

                if (row != null)
                {

                    /*  2.  Send the response informing whether this existing row (email/phone/both) already registered
                     *      to either web/desktop/both services. Depending on these return flags, UI will raise a message :
                     *      
                     *      (IsRegisteredWeb && IsRegisteredDesktop) => "Email/Nomor telepon ini (depend on prefCom flag) telah didaftarkan untuk semua produk"
                     *      (!IsRegisteredWeb && IsRegisteredDesktop)
                     *          => "Email/Nomor telepon ini (depend on prefComm flag) saat ini telah didaftarkan untuk produk Neoclinic Desktop. Namun anda dapat mengecek email anda
                     *              apabila ingin melakukan registrasi produk Neoclinic Web"
                     *      (IsRegisteredWeb && !IsRegisteredDesktop)
                     *          => "Email/Nomor telepon ini (depend on prefComm flag) saat ini telah didaftarkan untuk produk Neoclinic Web. Namun administrator akan tetap menghubungi
                     *              anda melalui email/whatsapp untuk memastikan apakah anda ingin melakukan registrasi produk Neoclinic Desktop"
                     *      
                     */

                    /*  3.  Based on preferred product (1 Web/2 Desktop/3 Both) check both is_registered_web && is_registered_desktop 
                     *      so that in case both or one of them had registered and current request also refer to said product,
                     *      we don't need to bother re-generating Otp again.
                     */

                    bool needsOtpForWeb = (product == ProductTypes.Web || product == ProductTypes.Both) && !row.IsRegisteredWeb;
                    if (needsOtpForWeb)
                    {
                        OtpAndExpiry = DoGenerateHashedOtp();

                        await _query.UpdateOtpAsync(row, OtpAndExpiry.Item1, OtpAndExpiry.Item2, ct);

                        await _mail.SendNotifAsync(req.Email, product, OtpAndExpiry.Item1);

                        //if (prefComm == PrefComms.Phone || prefComm == PrefComms.Both)
                        //    await _notifier.SendWhatsappAsync(req.Phone, product, plainOtp);
                    }

                    await _mail.SendNotifAsync(req.Email, product, OtpAndExpiry.Item1);

                    response = GenerateResponse(PreRegistSaveResponse.Updated, prefComm, true, "Updated", row.IsRegisteredWeb, row.IsRegisteredDesktop);
                    return response;
                }

                //  2. Generate Otp character varying(255)
                OtpAndExpiry = DoGenerateHashedOtp();

                //  3. Store the Pre-registration data along with Otp
                await _query.AddAsync(req, OtpAndExpiry.Item1, OtpAndExpiry.Item2, ct);

                //  4. Mail/Send Whatsapp to the user or both depending on prefComm
                if (prefComm == PrefComms.Email || prefComm == PrefComms.Both)
                    await _mail.SendNotifAsync(req.Email, product, OtpAndExpiry.Item1);

                //if (prefComm == PrefComms.Phone || prefComm == PrefComms.Both)
                //    await _notifier.SendWhatsappAsync(req.Phone, product, newPlainOtp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General exception");
                response = GenerateResponse(PreRegistSaveResponse.Error, prefComm, false, "Internal Error", false, false);
            }

            response = GenerateResponse(PreRegistSaveResponse.Created, prefComm, true, "Created", false, false);

            //  6. Send return response to caller so UI can show the 
            return response;
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

        private PreRegistResponse GenerateResponse(PreRegistSaveResponse status, PrefComms prefComm, bool isSuccess = false, string message = "", bool isRegisteredWeb = false, bool isRegisteredDesktop = false)
            => new PreRegistResponse()
            {
                Success = isSuccess,
                Status = status,
                Message = message,
                PrefComm = prefComm,
                IsRegisteredWeb = isRegisteredWeb,
                IsRegisteredDesktop = isRegisteredDesktop
            };
    }
}
