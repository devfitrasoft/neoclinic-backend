using neo.admin.Data.Entities;
using neo.admin.Data;
using neo.admin.Models;
using neo.admin.Services;
using Microsoft.EntityFrameworkCore;

public sealed class RegistrationService
{
    private readonly EnterpriseDbContext _db;
    private readonly IDBManagementClient _dbMgmt;
    private readonly RegistrationMailService _mail;
    private readonly ICaptchaValidator _captcha;

    public RegistrationService(
        EnterpriseDbContext db,
        IDBManagementClient dbMgmt,
        RegistrationMailService mail,
        ICaptchaValidator captcha)
        => (_db, _dbMgmt, _mail, _captcha) = (db, dbMgmt, mail, captcha);

    public async Task<RegisterFaskesResponse> RegisterAsync(RegisterFaskesRequest req, CancellationToken ct)
    {
        if (!await _captcha.VerifyAsync(req.CaptchaToken, ct))
            throw new InvalidOperationException("CAPTCHA failed");

        // 1. Upsert corporate (only if corporate mode is on)
        Corporate? corp = null;
        if (req.IsCorporate)
        {
            corp = req.CorporateId != null
                ? await _db.Corporates.FindAsync([req.CorporateId.Value], ct)
                : await CreateCorporateIfMissing(req.CorporateName!, ct);
        }

        // 2. Lookup faskes by NoFaskes (not Name!)
        var faskes = await _db.Faskeses
                        .FirstOrDefaultAsync(f => f.NoFaskes == req.NoFaskes, ct);

        if (faskes == null)
            faskes = await CreateNewFaskes(req, corp, ct);

        // 3. Create SU login if it doesn't already exist
        string username = $"{faskes.NoFaskes}.SU";

        var login = await _db.Logins
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

            _db.Logins.Add(login);

            try
            {
                await _db.SaveChangesAsync(ct);     // persists both new faskes (if any) and login
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate") == true)
            {
                // Another request beat us; re‑query the existing login
                login = await _db.Logins
                                .FirstOrDefaultAsync(l => l.Username == username, ct)
                                ?? throw new InvalidOperationException("Login not found");          // re‑throw if truly unexpected
            }
        }

        // 4. Ask user to confirm payment
        await _mail.SendConfirmPaymentReminder(req.Email, faskes.Name);

        // 5. Only return true when both were newly inserted (Id > 0)
        bool success = faskes.Id > 0 && login.Id > 0;

        return new RegisterFaskesResponse(success);
    }

    private async Task<Corporate> CreateCorporateIfMissing(string name, CancellationToken ct)
    {
        var upper = name.ToUpperInvariant();

        var corp = await _db.Corporates
                            .FirstOrDefaultAsync(c => c.Name == upper, ct);

        if (corp != null) return corp;

        corp = new Corporate
        {
            Name = upper,
            CreatedAt = DateTime.UtcNow,
            CreatorId = 0
        };
        _db.Corporates.Add(corp);
        await _db.SaveChangesAsync(ct);
        return corp;
    }

    private async Task<Faskes> CreateNewFaskes(RegisterFaskesRequest req, Corporate? corp, CancellationToken ct)
    {
        // Double-check again to avoid race condition
        var existing = await _db.Faskeses
                                .FirstOrDefaultAsync(f => f.NoFaskes == req.NoFaskes, ct);
        if (existing != null)
            return existing;

        var faskes = new Faskes
        {
            NoFaskes = req.NoFaskes,
            Name = req.Name,
            CorporateId = corp?.Id,
            Corporate = corp,
            Email = req.Email,
            PhoneNumber = req.Phone,
            Address = req.Address,
            RegisteredDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatorId = 0
        };

        _db.Faskeses.Add(faskes);
        await _db.SaveChangesAsync(ct);

        return faskes;
    }

}
