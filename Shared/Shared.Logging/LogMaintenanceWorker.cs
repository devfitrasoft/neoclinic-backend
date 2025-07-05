using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Logging;

public sealed class LogMaintenanceWorker : BackgroundService
{
    private readonly ILogger<LogMaintenanceWorker> _log;
    private readonly IConfiguration _cfg;
    private readonly string _logDir;
    private readonly int _daysToKeep;
    private readonly string? _sevenZipExe;

    public LogMaintenanceWorker(IConfiguration cfg,
                                ILogger<LogMaintenanceWorker> log)
    {
        _cfg = cfg;
        _log = log;
        _logDir = Path.GetDirectoryName(cfg["Logging:FilePath"] ?? Path.Combine(LogConstants.Dir, LogConstants.Ext)) ?? LogConstants.Dir;
        _daysToKeep = cfg.GetValue<int>("Logging:RetentionDays", LogConstants.Default_Retention); // Keep 2 month's old files by default
        _sevenZipExe = cfg["Logging:SevenZipExe"];
    }

    protected override async Task ExecuteAsync(CancellationToken stop)
    {
        while (!stop.IsCancellationRequested)
        {
            try
            {
                CompressYesterday();
                PurgeOldArchives();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Log maintenance failed");
            }

            await Task.Delay(TimeSpan.FromHours(24), stop);
        }
    }

    private void CompressYesterday()
    {
        if (string.IsNullOrWhiteSpace(_sevenZipExe) || !File.Exists(_sevenZipExe))
        {
            _log.LogWarning("SevenZipExe not configured or not found. Skipping compression step.");
            return;
        }

        var yesterday = DateTime.UtcNow.AddDays(-1).ToString("yyyyMMdd");
        var txt = Path.Combine(_logDir, $"{yesterday}.txt");
        var seven = Path.Combine(_logDir, $"{yesterday}.7z");

        if (!File.Exists(txt) || File.Exists(seven)) return;

        Directory.CreateDirectory(_logDir);

        var psi = new ProcessStartInfo
        {
            FileName = _sevenZipExe,
            Arguments = $"a -bb0 -t7z \"{seven}\" \"{txt}\" -mx=9",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            UseShellExecute = false
        };

        using var proc = Process.Start(psi)!;
        proc.WaitForExit();

        if (proc.ExitCode != 0)
        {
            _log.LogError("7‑Zip exited with code {Code}: {Err}",
                          proc.ExitCode, proc.StandardError.ReadToEnd());
            return;
        }

        File.Delete(txt);
        _log.LogInformation("Archived {Txt} → {Seven}", txt, seven);
    }

    private void PurgeOldArchives()
    {
        var cutoff = DateTime.UtcNow.AddDays(-_daysToKeep);

        foreach (var file in Directory.EnumerateFiles(_logDir, "*.7z"))
        {
            if (File.GetCreationTimeUtc(file) < cutoff)
            {
                File.Delete(file);
                _log.LogInformation("Deleted old archive {File}", file);
            }
        }
    }
}
