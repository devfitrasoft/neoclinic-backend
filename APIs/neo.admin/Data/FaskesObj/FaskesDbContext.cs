// APIs/neo.admin/Data/Clinic/FaskesDbContext.cs
using Microsoft.EntityFrameworkCore;
using Shared.Entities.Objs.FaskesObj;
using Shared.Entities.Queries;

namespace neo.admin.Data.FaskesObj;

public class FaskesDbContext : DbContext, IFaskesDbContext
{
    public FaskesDbContext(DbContextOptions<FaskesDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserFaskes> UserFaskeses => Set<UserFaskes>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Auth> Auths => Set<Auth>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Menu> Menus => Set<Menu>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        /* --- sys_role -------------------------------------------------- */
        b.Entity<Role>(e =>
        {
            e.Property(r => r.Id)
             .UseIdentityByDefaultColumn();

            e.HasIndex(r => r.Name).IsUnique();
            e.Property(r => r.Name).HasMaxLength(50);
        });

        /* --- sys_module ------------------------------------------------ */
        b.Entity<Module>(e =>
        {
            e.Property(mo => mo.Id)
             .UseIdentityByDefaultColumn();

            e.HasIndex(mo => mo.Code).IsUnique(); // Important for FK
            e.Property(mo => mo.Code).HasMaxLength(2).IsRequired();
            e.Property(mo => mo.Name).HasMaxLength(50);

            // Tell EF that 'Code' is the alternate key (used for relationships)
            e.HasAlternateKey(mo => mo.Code);
        });

        /* --- sys_group ------------------------------------------------- */
        b.Entity<Group>(e =>
        {
            e.Property(g => g.Id)
             .UseIdentityByDefaultColumn();

            e.HasIndex(g => g.Code).IsUnique(); // Important for FK
            e.Property(g => g.Code).HasMaxLength(2).IsRequired();

            // Tell EF that 'Code' is the alternate key (used for relationships)
            e.HasAlternateKey(g => g.Code);

            e.HasOne(g => g.Module)
             .WithMany()
             .HasForeignKey(g => g.ModuleCode)
             .HasPrincipalKey(mo => mo.Code)
             .OnDelete(DeleteBehavior.Restrict); // FK points to Module.Code

            e.Property(g => g.Name).HasMaxLength(50);
        });

        /* --- sys_menu -------------------------------------------------- */
        b.Entity<Menu>(e =>
        {
            e.Property(a => a.Id)
             .UseIdentityByDefaultColumn();              // auto‑increment (int)

            e.HasIndex(m => m.Code).IsUnique();
            e.Property(m => m.Name).HasMaxLength(50);

            e.HasOne(m => m.Module)
             .WithMany()
             .HasForeignKey(m => m.ModuleCode)
             .HasPrincipalKey(mo => mo.Code)
             .OnDelete(DeleteBehavior.Restrict); // FK points to Module.Code;

            e.HasOne(m => m.Group)
             .WithMany()
             .HasForeignKey(m => m.GroupCode)
             .HasPrincipalKey(g => g.Code)
             .OnDelete(DeleteBehavior.Restrict); // FK points to Group.Code;
        });

        /* --- sys_auth -------------------------------------------------- */
        b.Entity<Auth>(e =>
        {
            e.Property(a => a.Id)
             .UseIdentityByDefaultColumn();              // auto‑increment (int)

            e.HasOne(a => a.Role)
             .WithMany(r => r.Auths)
             .HasForeignKey(a => a.RoleId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(a => a.Module)
             .WithMany()
             .HasForeignKey(a => a.ModuleCode)
             .HasPrincipalKey(mo => mo.Code)
             .OnDelete(DeleteBehavior.Restrict); // FK points to Module.Code;

            e.HasOne(a => a.Group)
             .WithMany()
             .HasForeignKey(a => a.GroupCode)
             .HasPrincipalKey(g => g.Code)
             .OnDelete(DeleteBehavior.Restrict); // FK points to Group.Code;

            e.HasOne(a => a.Menu)
             .WithMany()
             .HasForeignKey(a => a.MenuCode)
             .HasPrincipalKey(m => m.Code)
             .OnDelete(DeleteBehavior.Restrict); // FK points to Group.Code;

            e.HasIndex(a => new { a.RoleId, a.ModuleCode, a.GroupCode, a.MenuCode }).IsUnique();
        });

        /* --- sys_user -------------------------------------------------- */
        b.Entity<User>(e =>
        {
            e.Property(u => u.Id)
             .UseIdentityByDefaultColumn();              // auto‑increment (int)

            e.HasOne(u => u.Role)
             .WithMany()
             .HasForeignKey(u => u.RoleId)
             .OnDelete(DeleteBehavior.Restrict);
            // Login nav is cross‑database; configure as FK only if you use the same server
            e.Ignore(u => u.Login);
        });

        /* --- sys_user_role -------------------------------------------- */
        b.Entity<UserFaskes>(e =>
        { 
            e.HasKey(uf => uf.Id);
        });
    }
}
