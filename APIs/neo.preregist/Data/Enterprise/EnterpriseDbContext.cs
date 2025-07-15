using Microsoft.EntityFrameworkCore;
using neo.preregist.Data.Enterprise.Entities;

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

                e.Property(x => x.PreferredContact)
                 .HasColumnType("smallint");

                e.Property(x => x.ProductType)
                 .HasColumnType("smallint");
            });
        }
    }
}
