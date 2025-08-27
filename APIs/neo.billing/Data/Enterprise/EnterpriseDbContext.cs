using Microsoft.EntityFrameworkCore;
using Shared.Entities.Objs.Enterprise;
using Shared.Entities.Queries;

namespace neo.scheduler.Data.Enterprise
{
    public class EnterpriseDbContext : DbContext, IOtpTokenDbContext
    {
        public EnterpriseDbContext(DbContextOptions options) : base(options) { }

        public DbSet<OtpToken> OtpTokens => Set<OtpToken>();

        public DbSet<PreRegist> PreRegists => Set<PreRegist>();

        public async Task ExecuteRawSqlAsync(string sql, CancellationToken ct = default)
        {
            await Database.ExecuteSqlRawAsync(sql, ct);
        }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);
        }
    }
}
