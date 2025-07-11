using Microsoft.EntityFrameworkCore;
using neo.admin.Data.Enterprise;
using neo.admin.Data.Enterprise.Entities;
using neo.admin.Data.Services;
using neo.admin.Models;
using neo.admin.Services;
using Shared.Common;

namespace neo.admin.Facades
{
    public interface IRegistrationFacade
    {
        Task<RegisterFaskesResponse> RegisterAsync(RegisterFaskesRequest req, CancellationToken ct);
        Task ActivateFaskesAsync(string loginUsername, CancellationToken ct = default);
    }

    public sealed class RegistrationFacade : IRegistrationFacade
    {
        private readonly IConfiguration _cfg;
        private readonly EnterpriseDbContext _edb;
        private readonly FaskesQueryService _faskes;
        private readonly MailService _mail;
        private readonly CorporateQueryService _corporate;
        private readonly FaskesDbProvisionerService _prov;

        private readonly ICaptchaValidator _captcha;

        public RegistrationFacade(
            IConfiguration cfg,
            EnterpriseDbContext edb,
            FaskesQueryService faskes,
            FaskesDbProvisionerService prov,
            CorporateQueryService corporate,
            MailService mail,
            ICaptchaValidator captcha) { 
            _cfg = cfg;
            _edb = edb;
            _mail = mail;
            _prov = prov;
            _faskes = faskes;
            _corporate = corporate;
            _captcha = captcha;
        }

        /// <summary>
        /// Perform several actions:<br/>
        /// 1. Register new corporate IF new faskes is part of corporation and said corporate has yet to exist.<br/>
        /// 2. Check and return state if nomor faskes were already registered.<br/>
        /// 3. Create new faskes and new login if said objects have yet to exist.<br/>
        /// 4. Send email to registrant asking for payment confirmation.<br/>
        /// 5. Send response to caller informing whether both faskes and login account have been registered successfully or not
        /// </summary>
        /// <param name="req"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<RegisterFaskesResponse> RegisterAsync(RegisterFaskesRequest req, CancellationToken ct)
        {
            if (!await _captcha.VerifyAsync(req.CaptchaToken, ct))
                throw new InvalidOperationException("CAPTCHA failed");

            // 1. Upsert corporate (only if corporate mode is on)
            Corporate? corp = null;
            if (req.IsCorporate)
            {
                corp = req.CorporateId != null
                    ? await _edb.Corporates.FindAsync([req.CorporateId.Value], ct)
                    : await _corporate.CreateCorporateIfMissing(req.CorporateName!, ct);
            }

            bool preExisted = false;
            bool success = false;
            // 2. Lookup faskes by NoFaskes
            var faskes = await _edb.Faskeses
                            .FirstOrDefaultAsync(f => f.NoFaskes == req.NoFaskes, ct);

            if (faskes == null)
            {
                faskes = await _faskes.CreateNewFaskes(req, corp, ct);
            }
            else
            {
                success = preExisted = true;  // mark this faskes as pre-existed
                return new RegisterFaskesResponse(success, preExisted);
            }

            // 3. Create SU login if it doesn't already exist
            string username = $"{faskes.NoFaskes}.SU";

            var login = await _edb.Logins
                                 .FirstOrDefaultAsync(l => l.Username == username, ct);

            if (login == null)
            {
                login = new Login
                {
                    Username = username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                    Faskes = faskes,              // assuming faskes is tracked
                    CorporateId = corp?.Id,
                    Email = req.Email,
                    PhoneNumber = req.Phone,
                    CreatedAt = DateTime.UtcNow,
                    CreatorId = 0
                };

                _edb.Logins.Add(login);

                try
                {
                    await _edb.SaveChangesAsync(ct);     // persists both new faskes (if any) and login
                }
                catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate") == true)
                {
                    // Another request beat us; re‑query the existing login
                    login = await _edb.Logins
                                    .FirstOrDefaultAsync(l => l.Username == username, ct)
                                    ?? throw new InvalidOperationException("Login not found");          // re‑throw if truly unexpected
                }
            }

            // 4. Ask user to confirm payment
            await _mail.SendConfirmPaymentReminder(req.Email, faskes.Name);

            // 5. Only return true when both were newly inserted (Id > 0)
            success = faskes.Id > 0 && login.Id > 0;

            return new RegisterFaskesResponse(success, preExisted);
        }
    
        public async Task ActivateFaskesAsync(string loginUsername, CancellationToken ct = default)
        {
            /* 1. parse "<nofaskes>.SU" */
            var segments = StringParser.DivideToSegmentsByDots(loginUsername);
            string noFaskes = segments[0]; 

            /* 2. fetch login & faskes */
            var login = await _edb.Logins.Include(l => l.Faskes)
                                         .Include(l => l.Corporate)
                                         .FirstOrDefaultAsync(l => l.Username == loginUsername, ct)
                       ?? throw new InvalidOperationException("Login not found");

            var faskes = login.Faskes ?? throw new InvalidOperationException("Faskes missing");

            /* 3. provision clinic DB (creates, migrates, seeds) */
            await _prov.ProvisionAsync(noFaskes, login.Id, faskes.Name, ct);

            /* 4. insert sys_connstring if missing */
            if (!await _edb.ConnStrings.AnyAsync(c => c.LoginId == login.Id, ct))
            {
                var cstringDefaults = _cfg.GetSection("ClinicDbDefaults");
                _edb.ConnStrings.Add(new ConnString
                {
                    LoginId = login.Id,
                    DbName = $"db_neoclinic_{faskes.Id:D8}",
                    DbHost = cstringDefaults.GetValue("Host", Constants.DB_FASKES_DEFAULT_HOST),
                    DbUsername = cstringDefaults.GetValue("Username", Constants.DB_FASKES_DEFAULT_USERNAME),
                    DbPassword = cstringDefaults.GetValue("Password", Constants.DB_FASKES_DEFAULT_PASSWORD),
                    CreatedAt = DateTime.UtcNow,
                    CreatorId = login.Id
                });
                await _edb.SaveChangesAsync(ct);
            }

            /* 5. send SU invite mail */
            await _mail.SendInviteAsync(faskes.Email, faskes.Id, login.Id);
        }
    }
}
