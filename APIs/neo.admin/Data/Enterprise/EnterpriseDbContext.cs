using Microsoft.EntityFrameworkCore;
using neo.admin.Data.Enterprise.Entities;

namespace neo.admin.Data.Enterprise
{
    public class EnterpriseDbContext : DbContext
    {
        public EnterpriseDbContext(DbContextOptions<EnterpriseDbContext> options) : base(options) { }

        public DbSet<Corporate> Corporates => Set<Corporate>();
        public DbSet<Faskes> Faskeses => Set<Faskes>();   // EF pluralises badly, explicit set
        public DbSet<Login> Logins => Set<Login>();
        public DbSet<ConnString> ConnStrings => Set<ConnString>();

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
        }
    }
}
