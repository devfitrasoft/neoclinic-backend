using Microsoft.EntityFrameworkCore;
using neo.admin.Data.Enterprise;
using neo.admin.Models;
using Shared.Entities.Enterprise;

namespace neo.admin.Queries
{
    public class LoginQueries
    {
        private readonly EnterpriseDbContext _edb;

        public LoginQueries(EnterpriseDbContext edb) => _edb = edb;

        public async Task<Login?> GetLoginByUsernameAsync(string username, CancellationToken ct)
            => await _edb.Logins.FirstOrDefaultAsync(l => l.Username == username, ct);

        public async Task<Login?> GetLoginFaskesCorpByUsernameAsync(string username, CancellationToken ct)
            => await _edb.Logins.Include(l => l.Faskes)
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
    }
}
