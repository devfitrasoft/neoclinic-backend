﻿using neo.admin.Data.Enterprise;
using neo.admin.Data.FaskesObj.Factories;
using neo.admin.Seeders;

namespace neo.admin.Migrations.Factories
{
    public class DbProvisionerFactory
    {
        private readonly EnterpriseDbContext _edbctx;
        private readonly FaskesDbContextFactory _faskesFactory;

        public DbProvisionerFactory(FaskesDbContextFactory faskesFactory, EnterpriseDbContext edbctx)
        {
            _edbctx = edbctx;
            _faskesFactory = faskesFactory;
        }

        /// <param name="loginId">Enterprise DB sys_login.Id of the SU account.</param>
        public async Task ProvisionAsync(string noFaskes, long loginId, string clinicName, CancellationToken ct = default)
        {
            await using var cdb = _faskesFactory.CreateAndMigrate(noFaskes);    // Create db_neoclinic_noFaskes & Ensure tables.

            //  Seed first row for SUPER USER Role
            int suRoleId = await FaskesSeeder.EnsureSuperUserRoleAsync(cdb, ct);    // Ensure the first login account is tied into "SUPER USER" role
            await FaskesSeeder.EnsureSuperUserAccountAsync(cdb, loginId, suRoleId, clinicName, ct); // Ensure the first login account has it's user information tied
            await FaskesSeeder.EnsureSuperUserDefaultFaskesAsync(cdb, _edbctx, loginId, noFaskes, ct);  // Ensure the first login account is tied to it's registration's faskes
        }
    }
}
