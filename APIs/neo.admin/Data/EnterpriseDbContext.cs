using Microsoft.EntityFrameworkCore;
using neo.admin.Data.Entities;

namespace neo.admin.Data
{
    public class EnterpriseDbContext : DbContext
    {
        public EnterpriseDbContext(DbContextOptions<EnterpriseDbContext> options) : base(options) { }

        public DbSet<Corporate> Corporates => Set<Corporate>();
        public DbSet<Faskes> Faskeses => Set<Faskes>();   // EF pluralises badly, explicit set
        public DbSet<Login> Logins => Set<Login>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // ---- sys_corporate ---------------------------------
            b.Entity<Corporate>(e =>
            {
                e.HasIndex(c => c.Name).IsUnique();
            });

            // ---- sys_faskes ------------------------------------
            b.Entity<Faskes>(e =>
            {
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
                e.HasKey(x => x.Id);
                e.Property(x => x.Id)
                 .HasColumnName("id")
                 .UseIdentityByDefaultColumn();              // auto‑increment (bigserial)

                e.Property(x => x.LoginId).HasColumnName("login_id").IsRequired();
                e.HasOne(x => x.Login)
                 .WithMany()
                 .HasForeignKey(x => x.LoginId);

                e.Property(x => x.DbName).HasColumnName("db_name").HasColumnType("varchar(255)");
                e.Property(x => x.DbHost).HasColumnName("db_host").HasColumnType("varchar(50)");
                e.Property(x => x.DbUsername).HasColumnName("db_username").HasColumnType("varchar(255)");
                e.Property(x => x.DbPassword).HasColumnName("db_password").HasColumnType("varchar(255)");
            });
        }
    }
}
