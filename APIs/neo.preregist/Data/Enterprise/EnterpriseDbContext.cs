using Microsoft.EntityFrameworkCore;
using Shared.Entities.Enterprise;

namespace neo.preregist.Data.Enterprise
{
    public class EnterpriseDbContext : DbContext
    {
        public EnterpriseDbContext(DbContextOptions<EnterpriseDbContext> options) : base(options) { }

        public DbSet<PreRegist> PreRegists => Set<PreRegist>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // ---- pre_regist ---------------------------------
            b.Entity<PreRegist>(e =>
            {
                e.Property(x => x.Id)
                 .UseIdentityByDefaultColumn();              // auto‑increment (bigserial)
            });
        }
    }
}
