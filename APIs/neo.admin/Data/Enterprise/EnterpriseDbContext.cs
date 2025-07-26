using Microsoft.EntityFrameworkCore;
using Shared.Entities.Objs.Enterprise;
using Shared.Entities.Queries;

namespace neo.admin.Data.Enterprise
{
    public class EnterpriseDbContext : DbContext, IEnterpriseDbContext, IPreRegistDbContext, 
        IOtpTokenDbContext, IBillingDbContext, IPICDbContext
    {
        public EnterpriseDbContext(DbContextOptions<EnterpriseDbContext> options) : base(options) { }

        public DbSet<Corporate> Corporates => Set<Corporate>();
        public DbSet<Faskes> Faskeses => Set<Faskes>();   // EF pluralises badly, explicit set
        public DbSet<Login> Logins => Set<Login>();
        public DbSet<ConnString> ConnStrings => Set<ConnString>();
        public DbSet<PreRegist> PreRegists => Set<PreRegist>();
        public DbSet<OtpToken> OtpTokens => Set<OtpToken>();
        public DbSet<AuthSession> AuthSessions => Set<AuthSession>();

        public DbSet<Billing> Billings => Set<Billing>();
        public DbSet<BillingSetting> BillingsSettings => Set<BillingSetting>();

        public DbSet<PIC> PICs => Set<PIC>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // ---- sys_corporate ---------------------------------
            b.Entity<Corporate>(e =>
            {
                e.Property(c => c.Id)
                 .UseIdentityByDefaultColumn();

                e.HasIndex(c => c.Name).IsUnique();
            });

            // ---- sys_faskes ------------------------------------
            b.Entity<Faskes>(e =>
            {
                e.Property(f => f.Id)
                 .UseIdentityByDefaultColumn();

                e.HasIndex(f => f.NoFaskes).IsUnique();
                e.HasIndex(f => f.Name);
                e.HasOne(f => f.Corporate)
                  .WithMany(c => c.Faskes)
                  .HasForeignKey(f => f.CorporateId)
                  .OnDelete(DeleteBehavior.Restrict);

                e.HasMany(f => f.Billings)
                 .WithOne(b => b.Faskes)
                 .HasForeignKey(b => b.FaskesId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasMany(f => f.PICs)
                 .WithOne(p => p.Faskes)
                 .HasForeignKey(b => b.FaskesId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ---- sys_login -------------------------------------
            b.Entity<Login>(e =>
            {
                e.Property(l => l.Id)
                 .UseIdentityByDefaultColumn();

                e.HasIndex(l => l.Username).IsUnique();
                e.HasIndex(l => l.Email);

                e.HasOne(l => l.Faskes)
                  .WithMany(f => f.Logins)
                  .HasForeignKey(l => l.FaskesId)
                  .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(l => l.Corporate)
                  .WithMany()
                  .HasForeignKey(l => l.CorporateId)
                  .OnDelete(DeleteBehavior.Restrict);
            });

            // ---- sys_connstring ---------------------------------
            b.Entity<ConnString>(e =>
            {
                e.Property(x => x.Id)
                 .UseIdentityByDefaultColumn();              // auto‑increment (bigserial)

                e.Property(x => x.LoginId)
                 .IsRequired();

                e.HasOne(x => x.Login)
                 .WithMany()
                 .HasForeignKey(x => x.LoginId);
            });

            // ---- sys_billing ------------------------------------
            b.Entity<Billing>(e => 
            { 
                e.Property(x => x.Id) 
                 .UseIdentityByDefaultColumn();

                e.HasOne(b => b.Faskes)
                 .WithMany(f => f.Billings)
                 .HasForeignKey(b => b.FaskesId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ---- sys_billing_setting ----------------------------
            b.Entity<BillingSetting>(e =>
            {
                e.Property(s => s.Id)
                 .UseIdentityByDefaultColumn();

                e.HasIndex(s => s.IsActive); // for easy lookup of current active rule
            });

            // ---- sys_pic ----------------------------------------
            b.Entity<PIC>(e =>
            {
                e.Property(x => x.Id)
                 .UseIdentityByDefaultColumn();

                e.Property(x => x.PICType)
                 .HasColumnType("smallint");

                e.HasOne(p => p.Faskes)
                 .WithMany(f => f.PICs)
                 .HasForeignKey(p => p.FaskesId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
