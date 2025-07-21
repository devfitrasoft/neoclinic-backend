using Microsoft.EntityFrameworkCore;
using Shared.Entities.Objs.Enterprise;
using Shared.Entities.Queries;

namespace neo.preregist.Data.Enterprise
{
    public class EnterpriseDbContext : DbContext, IPreRegistDbContext, IOtpTokenDbContext
    {
        public EnterpriseDbContext(DbContextOptions<EnterpriseDbContext> options) : base(options) { }

        public DbSet<PreRegist> PreRegists => Set<PreRegist>();
        public DbSet<OtpToken> OtpTokens => Set<OtpToken>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // ---- pre_regist ---------------------------------
            b.Entity<PreRegist>(e =>
            {
                e.Property(x => x.Id)
                 .UseIdentityByDefaultColumn();              // auto‑increment (bigserial)
            });

            b.Entity<OtpToken>(e =>
            {
                e.Property(o => o.Type)
                 .HasColumnType("smallint");
            });
        }
    }
}
