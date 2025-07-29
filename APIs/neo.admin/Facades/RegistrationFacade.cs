using neo.admin.Data.Enterprise;
using neo.admin.Migrations.Factories;
using neo.admin.Models;
using neo.admin.Services;
using Org.BouncyCastle.Ocsp;
using Serilog;
using Shared.Common;
using Shared.Entities.Objs.Enterprise;
using Shared.Entities.Queries.Enterprise;
using Shared.Models;

namespace neo.admin.Facades
{
    public interface IRegistrationFacade
    {
        Task<RegisterFaskesResponseData> RegisterAsync(RegisterFaskesRequest req, CancellationToken ct);
        Task<bool> ActivateFaskesAsync(string loginUsername, CancellationToken ct = default);
    }

    public sealed class RegistrationFacade : IRegistrationFacade
    {
        private readonly IConfiguration _cfg;
        private readonly ILogger<RegistrationFacade> _logger;

        private readonly MailService _mail;

        private readonly PICQueries _picQry;
        private readonly LoginQueries _loginQry;
        private readonly OtpTokenQueries _otpQry;
        private readonly FaskesQueries _faskesQry;
        private readonly BillingQueries _billingQry;
        private readonly CorporateQueries _corpQry;
        private readonly ConnStringQueries _cstrQry;
        private readonly PreRegistQueries _preRegistQry;
        private readonly BillingSettingQueries _billingSettingQry;

        private readonly DbProvisionerFactory _prov;

        private readonly ICaptchaValidatorService _captcha;

        public RegistrationFacade(ILogger<RegistrationFacade> logger, IConfiguration cfg, EnterpriseDbContext edb, DbProvisionerFactory prov,
            MailService mail, ICaptchaValidatorService captcha, OtpTokenQueries otpQry, PreRegistQueries preRegistQry,
            LoginQueries loginQry, FaskesQueries faskesQry, CorporateQueries corpQry, PICQueries picQry, BillingSettingQueries billingSettingQry,
            BillingQueries billingQry) 
        {
            _cfg = cfg;
            _logger = logger;

            _mail = mail;

            _prov = prov;

            _otpQry = otpQry;
            _picQry = picQry;
            _corpQry = corpQry;
            _loginQry = loginQry;
            _faskesQry = faskesQry;
            _billingQry = billingQry;
            _preRegistQry = preRegistQry;
            _billingSettingQry = billingSettingQry;
            _cstrQry = new ConnStringQueries(cfg, edb); // it's loaded by every faskes db generation

            _captcha = captcha;
        }

        /// <summary>
        /// Perform several actions:<br/>
        /// 1. Register new corpQry IF new faskesQry is part of corporation and said corpQry has yet to exist.<br/>
        /// 2. Check and return state if nomor faskesQry were already registered.<br/>
        /// 3. Create new faskesQry and new login if said objects have yet to exist.<br/>
        /// 4. Send email to registrant asking for payment confirmation.<br/>
        /// 5. Send response to caller informing whether both faskesQry and login account have been registered successfully or not
        /// </summary>
        /// <param name="req"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<RegisterFaskesResponseData> RegisterAsync(RegisterFaskesRequest req, CancellationToken ct)
        {
            await _otpQry.MarkIsUsedAsync(req.Otp, OtpType.PreRegist, ct);

            if (!await _captcha.VerifyAsync(req.CaptchaToken, ct))
                _logger.LogError("RegisterAsync: CAPTCHA failed");

            // 1. Upsert corpQry (only if corpQry mode is on)
            Corporate? corp = null;
            if (req.IsCorporate)
            {
                corp = req.CorporateId != null
                    ? await _corpQry.GetByIdAsync(req.CorporateId.Value, ct)
                    : await _corpQry.CreateCorporateIfMissing(req.CorporateName!, ct);
            }
            bool resCorp = req.IsCorporate ? corp != null : true;

            bool preExisted = false;
            bool success = false;
            // 2. Lookup faskesQry by NoFaskes
            var faskes = await _faskesQry.GetNotDeletedAsync(req.NoFaskes, ct);

            if (faskes == null)
            {
                faskes = await _faskesQry.AddNewAsync(req, corp, ct);
            }
            else
            {
                success = preExisted = true;  // mark this faskesQry as pre-existed
                return new RegisterFaskesResponseData(success, preExisted);
            }

            // 3. Insert all PICs (PJ, Billing, Technical)
            var addPjPICRes = await _picQry.AddAsync(faskes.Id, req.NamePj, req.EmailPj, req.PhonePj, PICCType.PJ, ct);
            var addBillPICRes = await _picQry.AddAsync(faskes.Id, req.NameBill, req.EmailBill, req.PhoneBill, PICCType.Billing, ct);
            var addTechPICRes = await _picQry.AddAsync(faskes.Id, req.NameTech, req.EmailTech, req.PhoneTech, PICCType.Tech, ct);

            // 4. Create SU login if it doesn't already exist
            string username = $"{faskes.NoFaskes}.SU";
            var login = await _loginQry.GenerateNewLoginIfNotExist(username, req, faskes, corp, ct);

            // 5. Create new registration billing
            var billingSetting = await _billingSettingQry.GetOrCreateActiveBillingSettingAsync(ct);
            var registerBilling = await _billingQry.GenerateRegistrationBillingAsync(faskes.Id, billingSetting, ct);

            // 5. Send email to user regarding initial billing
            await _mail.SendRegistrationFeeAsync(req.EmailPj, faskes.Name, billingSetting.RegistrationFee);

            // 6. Only return true when both were newly inserted (Id > 0)
            success = IsRegistrationSuccessful(resCorp, faskes.Id, addPjPICRes.Id, addBillPICRes.Id, 
                addTechPICRes.Id, login.Id, billingSetting.Id, registerBilling.Id);

            return new RegisterFaskesResponseData(success, preExisted);
        }
    
        public async Task<bool> ActivateFaskesAsync(string loginUsername, CancellationToken ct = default)
        {
            /* 1. parse "<nofaskes>.SU" */
            var segments = StringParser.DivideToSegmentsByDots(loginUsername);
            string noFaskes = segments[0]; 

            /* 2. fetch login & faskesQry */
            var login = await _loginQry.GetLoginFaskesCorpByUsernameAsync(loginUsername, ct);

            if (login == null) 
            {
                _logger.LogError("ActivateFaskesAsync: Login not found");
                return false;
            }

            var faskes = login.Faskes;

            var picPJ = faskes.PICs.FirstOrDefault(p => p.PICType == PICCType.PJ);
            var picBilling = faskes.PICs.FirstOrDefault(p => p.PICType == PICCType.Billing);
            var picTech = faskes.PICs.FirstOrDefault(p => p.PICType == PICCType.Tech);

            /* 3. grab registration billing by faskesId */
            var registrationBill = await _billingQry.GetRegistrationBillingByFaskesIdAsync(faskes.Id, ct);

            if (registrationBill == null)
            {
                _logger.LogInformation("No registration billing detected for this activation, re-generate new billing");

                /* 4. re-create new registration billing in-case this faskes doesn't have any billing */
                var billingSetting = await _billingSettingQry.GetOrCreateActiveBillingSettingAsync(ct);
                var registerBilling = await _billingQry.GenerateRegistrationBillingAsync(faskes.Id, billingSetting, ct);

                /* 5. re-send email to user regarding initial billing */
                await _mail.SendRegistrationFeeAsync(picPJ.Email, faskes.Name, billingSetting.RegistrationFee);

                /* 6. return immediately */
                return false;
            }

            /* 4. mark current registration billing as paid */
            int resMarkPaidBill = await _billingQry.MarkIsPaidTrueAsync(registrationBill, ct);

            /* 4. provision clinic DB (creates, migrates, seeds) */
            bool resProvision = await _prov.ProvisionAsync(noFaskes, login.Id, faskes.Name, ct);

            /* 5. insert sys_connstring if missing */
            int resCstring = await _cstrQry.GenerateByLoginIdIfMissing(login.Id, faskes.NoFaskes, ct);

            /* 6. update is_registered in db_neoclinic pre_regist for registered email or phone or both */
            int resUpdatePreRegist = picPJ == null ? 0 : await _preRegistQry.UpdatePreRegisteredFlagAsync(picPJ.Email, picPJ.Phone, ct);

            /* 7. update is_active for super user, faskes, corporate (if exist) and all PICs */
            int resIsActiveLogin = await _loginQry.UpdateIsActiveAsync(login, true, ct);
            int resIsActiveFaskes = await _faskesQry.UpdateIsActiveAsync(faskes, true, ct);
            int resIsActiveCorporate = faskes.Corporate == null ? 1 : await _corpQry.UpdateIsActiveAsync(faskes.Corporate, true, ct);
            int resIsActivePicPJ = picPJ == null ? 0 : await _picQry.UpdateIsActiveAsync(picPJ, true, ct);
            int resIsActivatePicBilling = picBilling == null ? 0 : await _picQry.UpdateIsActiveAsync(picBilling, true, ct);
            int resIsActivatePicTech = picTech == null ? 0 : await _picQry.UpdateIsActiveAsync(picTech, true, ct);

            /* 8. generate new otp for reset password */
            var OtpAndExpiry = Utilities.DoGenerateHashedOtp(_cfg["OtpToken :Expiry"]);
            int resOtp = await _otpQry.AddAsync(login.Id, OtpAndExpiry.Item1, OtpAndExpiry.Item2, OtpType.ResetPwd, ct);

            if(IsActivationSuccessful(resMarkPaidBill, resProvision, resCstring, resUpdatePreRegist, 
                resIsActiveLogin, resIsActiveFaskes, resIsActiveCorporate, 
                resIsActivePicPJ, resIsActivatePicBilling, resIsActivatePicTech, resOtp))
            {
                /* 9. send SU invite mail */
                await _mail.SendInviteAsync(picPJ.Email, login.Username, OtpAndExpiry);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsRegistrationSuccessful(bool resCorp, long faskesId, long addPjPICResId, long addBillPICResId, long addTechPICResId,
            long loginId, long billingSettingId, long registerBillingId)
            => resCorp && faskesId > 0 && addPjPICResId > 0 && addBillPICResId > 0 && addTechPICResId > 0 && loginId > 0 && billingSettingId > 0 && registerBillingId > 0;

        private bool IsActivationSuccessful(int resMarkPaidBill, bool resProvision, int resCstring, int resUpdatePreRegist, int resIsActiveLogin,
            int resIsActiveFaskes, int resIsActiveCorporate, int resIsActivePicPJ, int resIsActivatePicBilling, int resIsActivatePicTech,
            int resOtp)
            => resMarkPaidBill > 0 && resProvision && resCstring > 0 && resUpdatePreRegist > 0 && resUpdatePreRegist > 0 && resIsActiveLogin > 0
               && resIsActiveFaskes > 0 && resIsActiveCorporate > 0 && resIsActivePicPJ > 0 && resIsActivatePicBilling > 0
               && resIsActivatePicTech > 0 && resOtp > 0;
    }
}
