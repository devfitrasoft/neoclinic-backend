using Microsoft.EntityFrameworkCore;
using Shared.Entities.Objs.Enterprise;
using Shared.Models;

namespace Shared.Entities.Queries.Enterprise
{
    public sealed class LoginQueries
    {
        private readonly IEnterpriseDbContext _edb;

        public LoginQueries(IEnterpriseDbContext edb) => _edb = edb;

        public async Task<Login?> GetLoginByUsernameAsync(string username, CancellationToken ct)
            => await _edb.Logins.FirstOrDefaultAsync(l => l.Username == username, ct);

        public async Task<Login?> GetLoginFaskesCorpByUsernameAsync(string username, CancellationToken ct)
            => await _edb.Logins.Include(l => l.Faskes)
                                    .ThenInclude(f => f.PICs)
                                .Include(l => l.Corporate)
                                .FirstOrDefaultAsync(l => l.Username == username, ct);

        public async Task<Login> GenerateNewLoginIfNotExist(string username, RegisterFaskesRequest req, Faskes faskes, Corporate? corp, CancellationToken ct)
        {
            var login = await GetLoginByUsernameAsync(username, ct);

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

            return login;
        }

        public async Task<int> UpdateIsActiveAsync(Login login, bool isActive, CancellationToken ct)
        {
            login.IsActive = isActive;
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
