using Microsoft.EntityFrameworkCore;
using Shared.Entities.Objs.Enterprise;
using Shared.Models;

namespace Shared.Entities.Queries.Enterprise
{
    public sealed class LoginQueries
    {
        private readonly IEnterpriseDbContext _edb;

        public LoginQueries(IEnterpriseDbContext edb) => _edb = edb;

        public async Task<Login?> GetWithFaskesCorpByUsernameAsync(string username, CancellationToken ct) // For account activation
            => await _edb.Logins.Include(l => l.Faskes)
                                    .ThenInclude(f => f.PICs)
                                .Include(l => l.Corporate)
                                .FirstOrDefaultAsync(l => l.Username == username, ct);

        public async Task<Login?> GetActiveByUsernameAsync(string username, CancellationToken ct)
            => await _edb.Logins.Include(l => l.Faskes)
                                    .ThenInclude(f => f.PICs)
                                .Include(l => l.Corporate)
                                .FirstOrDefaultAsync(l => l.Username == username
                                                       && l.IsActive
                                                       && !l.IsDeleted, ct);

        public async Task<Login?> GetById(long id, CancellationToken ct)
            => await _edb.Logins.Include(l => l.Faskes)
                                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<Login> GenerateFaskesSUIfNotExist(string noFaskes, RegisterFaskesRequest req, Faskes faskes, Corporate? corp, CancellationToken ct)
        {
            string username = $"{faskes.NoFaskes}.SU";
            var login = await _edb.Logins.FirstOrDefaultAsync(l => l.Username == username, ct);

            if (login == null)
            {
                login = new Login
                {
                    Username = username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                    Faskes = faskes,              // assuming faskesQry is tracked
                    CorporateId = corp?.Id,
                    Email = req.Email,
                    PhoneNumber = req.Phone,
                    IsActive = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatorId = 0
                };

                _edb.Logins.Add(login);

                try
                {
                    await _edb.SaveChangesAsync(ct);     // persists both new faskesQry (if any) and login
                }
                catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate") == true)
                {
                    // Another request beat us; re‑query the existing login
                    login = await _edb.Logins
                                    .FirstOrDefaultAsync(l => l.Username == username, ct)
                                    ?? throw new InvalidOperationException("Login not found");          // re‑throw if truly unexpected
                }
            }
            else
            {
                // re-cycle existing login
                login.IsDeleted = false;
                login.IsActive = false;
                login.UpdatedAt = DateTime.UtcNow;
                login.CreatorId = 0;

                await _edb.SaveChangesAsync(ct);
            }

            return login;
        }

        public async Task<int> ActivateAsync(Login login, CancellationToken ct)
        {
            if (login.IsActive && !login.IsDeleted) return 1;

            login.IsActive = true;
            login.IsDeleted = false;
            return await _edb.SaveChangesAsync(ct);
        }

        public async Task<int> UpdatePasswordAsync(long userId, string hashedPassword, CancellationToken ct)
        {
            var user = await _edb.Logins.FindAsync(new object[] { userId }, ct);
            if (user != null)
            {
                user.PasswordHash = hashedPassword;
                return await _edb.SaveChangesAsync(ct);
            }

            return 0;
        }
    }
}
