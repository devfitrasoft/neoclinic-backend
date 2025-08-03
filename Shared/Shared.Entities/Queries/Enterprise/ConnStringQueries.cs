using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Common;
using Shared.Entities.Objs.Enterprise;

namespace Shared.Entities.Queries.Enterprise
{
    public sealed class ConnStringQueries
    {
        private readonly IConfiguration _cfg;
        private readonly IEnterpriseDbContext _edb;
        public ConnStringQueries(IConfiguration cfg, IEnterpriseDbContext edb)
        {
            _cfg = cfg;
            _edb = edb;
        }

        public async Task<int> GenerateByLoginIdIfMissing(long loginId, string noFaskes, CancellationToken ct)
        {
            if (!await _edb.ConnStrings.AnyAsync(c => c.LoginId == loginId, ct))
            {
                var cstringDefaults = _cfg.GetSection("ClinicDbDefaults");
                _edb.ConnStrings.Add(new ConnString
                {
                    LoginId = loginId,
                    DbName = $"db_neoclinic_{noFaskes:D8}",
                    DbHost = cstringDefaults.GetValue("Host", Constants.DB_FASKES_DEFAULT_HOST),
                    DbUsername = cstringDefaults.GetValue("Username", Constants.DB_FASKES_DEFAULT_USERNAME),
                    DbPassword = cstringDefaults.GetValue("Password", Constants.DB_FASKES_DEFAULT_PASSWORD),
                    CreatedAt = DateTime.UtcNow,
                    CreatorId = loginId
                });
                return await _edb.SaveChangesAsync(ct);
            }
            else
            {
                return 1;
            }
        }

        public async Task<ConnString?> GetByLoginId(long loginId, CancellationToken ct)
            => await _edb.ConnStrings.FirstOrDefaultAsync(r => r.LoginId == loginId, ct);
    }
}
