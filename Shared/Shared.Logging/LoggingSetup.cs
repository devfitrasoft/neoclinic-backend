// Shared.Logging/LoggingSetup.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;
using System.IO;

namespace Shared.Logging;

public static class LoggingSetup
{
    /// <summary>Create the very first logger so anything that fails during host‑building is still logged.</summary>

    public static void ConfigureBootstrapLogger(IConfiguration config)
    {
        var path = config["Logging:FilePath"] ?? Path.Combine(LogConstants.Dir, LogConstants.Ext);
        var enableFile = config.GetValue("Logging:EnableFileSink", true);

        var bootstrap = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(new RenderedCompactJsonFormatter());

        if (enableFile)
            bootstrap.WriteTo.File(path,
                                   rollingInterval: RollingInterval.Day,
                                   retainedFileCountLimit: null,
                                   outputTemplate: config["Logging:OutputTemplate"] ?? LogConstants.Output_Template);

        Log.Logger = bootstrap.CreateBootstrapLogger();
    }

    /// <summary>Attach Serilog to the generic host; honours appsettings + DI enrichers.</summary>
    public static void UseSerilogLogging(this IHostBuilder hostBuilder, IConfiguration cfg)
    {
        var path = cfg["Logging:FilePath"] ?? Path.Combine(LogConstants.Dir, LogConstants.Ext);
        var enableFile = cfg.GetValue("Logging:EnableFileSink", true);

        hostBuilder.UseSerilog((ctx, services, loggerCfg) =>
        {
            loggerCfg
                .ReadFrom.Configuration(ctx.Configuration)
                .ReadFrom.Services(services)
                .WriteTo.Console(new RenderedCompactJsonFormatter());

            if (enableFile)
                loggerCfg.WriteTo.File(path, rollingInterval: RollingInterval.Day);
        });
    }
}
