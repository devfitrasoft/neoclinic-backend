using Microsoft.EntityFrameworkCore;
using neo.admin.Data.Enterprise;
using Shared.Common;
using Shared.Entities.Enterprise;

namespace neo.admin.Queries
{
    public class ConnStringQueries
    {
        private readonly IConfiguration _cfg;
        private readonly EnterpriseDbContext _edb;
        public ConnStringQueries(IConfiguration cfg, EnterpriseDbContext edb)
        {
            _cfg = cfg;
            _edb = edb;
        }

        public async Task GenerateByLoginIdIfMissing(long loginId, long faskesId, CancellationToken ct)
        {
            if (!await _edb.ConnStrings.AnyAsync(c => c.LoginId == loginId, ct))
            {
                var cstringDefaults = _cfg.GetSection("ClinicDbDefaults");
                _edb.ConnStrings.Add(new ConnString
                {
                    LoginId = loginId,
                    DbName = $"db_neoclinic_{faskesId:D8}",
                    DbHost = cstringDefaults.GetValue("Host", Constants.DB_FASKES_DEFAULT_HOST),
                    DbUsername = cstringDefaults.GetValue("Username", Constants.DB_FASKES_DEFAULT_USERNAME),
                    DbPassword = cstringDefaults.GetValue("Password", Constants.DB_FASKES_DEFAULT_PASSWORD),
                    CreatedAt = DateTime.UtcNow,
                    CreatorId = loginId
                });
                await _edb.SaveChangesAsync(ct);
            }
        }
    }
}
