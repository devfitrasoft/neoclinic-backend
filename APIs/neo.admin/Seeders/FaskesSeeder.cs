﻿using neo.admin.Data.FaskesObj;
using Microsoft.EntityFrameworkCore;
using neo.admin.Data.Enterprise;
using Shared.Entities.Objs.FaskesObj;

namespace neo.admin.Seeders
{
    public class FaskesSeeder
    {
        /// <summary>
        /// Ensures “SUPER USER” role exists and returns its ID.
        /// </summary>
        public static async Task<int> EnsureSuperUserRoleAsync(FaskesDbContext cdb, CancellationToken ct)
        {
            var role = await cdb.Roles.FirstOrDefaultAsync(r => r.Name == "SUPER USER", ct);
            if (role != null) return role.Id;

            role = new Role
            {
                Name = "SUPER USER",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatorId = 0
            };
            cdb.Roles.Add(role);
            await cdb.SaveChangesAsync(ct);
            return role.Id;
        }

        /// <summary>
        /// Inserts first sys_user row for the given login ID if absent.
        /// </summary>
        public static async Task<int> EnsureSuperUserAccountAsync(
            FaskesDbContext cdb,
            long loginId,
            int roleId,
            string clinicName,
            CancellationToken ct)
        {
            bool exists = await cdb.Users.AnyAsync(u => u.LoginId == loginId, ct);
            if (exists) return 1;

            cdb.Users.Add(new User
            {
                LoginId = loginId,
                RoleId = roleId,
                Name = clinicName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatorId = loginId
            });
            return await cdb.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Inserts first sys_user_faskes row for the given login ID if absent (of which the faskesId will be the one said user is belong to).
        /// </summary>
        public static async Task<int> EnsureSuperUserDefaultFaskesAsync(
            FaskesDbContext cdb,
            EnterpriseDbContext edb,
            long loginId,
            string noFaskes,
            CancellationToken ct)
        {
            bool exists = await cdb.UserFaskeses.AnyAsync(u => u.LoginId == loginId, ct);
            if (exists) return 1;

            var Faskes = await edb.Faskeses.FirstOrDefaultAsync(u => u.NoFaskes == noFaskes, ct);
            if (Faskes == null) return 0;

            cdb.UserFaskeses.Add(new UserFaskes
            {
                LoginId = loginId,
                FaskesId = Faskes.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatorId = loginId
            });
            return await cdb.SaveChangesAsync(ct);
        }
    }
}
