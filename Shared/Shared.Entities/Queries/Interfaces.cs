using Microsoft.EntityFrameworkCore;
using Shared.Entities.Objs.Enterprise;
using Shared.Entities.Objs.FaskesObj;

namespace Shared.Entities.Queries
{
    public interface IEnterpriseDbContext
    {
        DbSet<PreRegist> PreRegists { get; }
        DbSet<Corporate> Corporates { get; }
        DbSet<Faskes> Faskeses { get; }
        DbSet<Login> Logins { get; }
        DbSet<ConnString> ConnStrings { get; }
        DbSet<AuthSession> AuthSessions { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }

    public interface IPreRegistDbContext
    {
        DbSet<PreRegist> PreRegists { get; }
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }

    public interface IOtpTokenDbContext
    {
        DbSet<OtpToken> OtpTokens { get; }
        DbSet<PreRegist> PreRegists { get; }
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }

    public interface IFaskesDbContext
    {
        DbSet<User> Users { get; }
        DbSet<UserFaskes> UserFaskeses { get; }
        DbSet<Role> Roles { get; }
        DbSet<Auth> Auths { get; }
        DbSet<Module> Modules { get; }
        DbSet<Group> Groups { get; }
        DbSet<Menu> Menus { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }

    public interface IPICDbContext
    {
        DbSet<PIC> PICs { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }

    public interface IBillingDbContext
    {
        DbSet<Billing> Billings { get; }
        DbSet<BillingSetting> BillingSettings { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
