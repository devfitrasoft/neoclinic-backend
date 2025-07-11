using neo.admin.Data.Enterprise;
using neo.admin.Data.FaskesObj.Factories;
using neo.admin.Seeders;

namespace neo.admin.Services
{
    public class FaskesDbProvisionerService
    {
        private readonly EnterpriseDbContext _edbctx;
        private readonly FaskesDbContextFactory _faskesFactory;

        public FaskesDbProvisionerService(FaskesDbContextFactory factory, EnterpriseDbContext edbctx)
        {
            _edbctx = edbctx;
            _faskesFactory = factory;
        }

        /// <param name="loginId">Enterprise DB sys_login.Id of the SU account.</param>
        public async Task ProvisionAsync(string noFaskes, long loginId, string clinicName, CancellationToken ct = default)
        {
            await using var cdb = _faskesFactory.CreateAndMigrate(noFaskes);

            int suRoleId = await FaskesSeeder.EnsureSuperUserRoleAsync(cdb, ct);    // Ensure the first login account is tied into "SUPER USER" role
            await FaskesSeeder.EnsureSuperUserAccountAsync(cdb, loginId, suRoleId, clinicName, ct); // Ensure the first login account has it's user information tied
            await FaskesSeeder.EnsureSuperUserDefaultFaskesAsync(cdb, _edbctx, loginId, noFaskes, ct);  // Ensure the first login account is tied to it's registration's faskes
        }
    }
}
