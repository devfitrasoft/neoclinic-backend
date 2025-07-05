// Shared.EFCore/AutoMigrationExtensions.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Shared.EFCore;

/// <summary>Convenience helpers for automatic EF‑Core migrations on app start.</summary>
public static class AutoMigrationExtensions
{
    /// <summary>
    /// Registers an <see cref="IStartupFilter"/> that will run
    /// <c>context.Database.Migrate()</c> during application startup.
    /// </summary>
    /// <typeparam name="TContext">Your <see cref="DbContext"/> type.</typeparam>
    /// <param name="services">The application's DI container.</param>
    /// <param name="markerTables">
    /// One or more table names that must exist to consider the schema "present".
    /// If none exist, migrations run; otherwise only pending migrations run.
    /// </param>
    public static IServiceCollection AddEfAutoMigration<TContext>(
        this IServiceCollection services,
        params string[] markerTables)
        where TContext : DbContext
    {
        services.AddSingleton<IStartupFilter>(
            new AutoMigrationStartupFilter<TContext>(markerTables));
        return services;
    }

    /* ---------- nested private class ---------- */

    private sealed class AutoMigrationStartupFilter<TContext> : IStartupFilter
        where TContext : DbContext
    {
        private readonly string[] _markers;
        public AutoMigrationStartupFilter(string[] markers) => _markers = markers;

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                using var scope = app.ApplicationServices.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<TContext>();

                try
                {
                    if (_markers.Length > 0 &&
                        db.Database.CanConnect() &&
                        db.Database.ExecuteSqlRaw(
                            $"SELECT 1 FROM pg_tables WHERE tablename IN ({string.Join(",", _markers.Select(t => $"'{t}'"))}) LIMIT 1") > 0)
                    {
                        Log.Information("{Context} schema detected; applying pending migrations only.", typeof(TContext).Name);
                    }
                    db.Database.Migrate();
                    Log.Information("{Context} migrations applied (if any).", typeof(TContext).Name);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Automatic EF migration failed for {Context}", typeof(TContext).Name);
                    throw;
                }

                next(app);
            };
        }
    }
}
