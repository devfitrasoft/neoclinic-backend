using Microsoft.EntityFrameworkCore;
using Shared.Entities.Objs.Enterprise;
using Shared.Entities.Queries;

namespace neo.billing.Data.Enterprise
{
    public class EnterpriseDbContext : DbContext, IBillingDbContext
    {
        public EnterpriseDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Billing> Billings => Set<Billing>();
        public DbSet<BillingSetting> BillingSettings => Set<BillingSetting>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);
        }
    }
}
