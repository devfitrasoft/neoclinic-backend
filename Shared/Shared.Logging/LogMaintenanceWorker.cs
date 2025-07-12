using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Logging;

public sealed class LogMaintenanceWorker : BackgroundService
{
    private readonly ILogger<LogMaintenanceWorker> _log;
    private readonly string _logDir;
    private readonly int _daysToKeep;
    private readonly string? _sevenZipExe;

    public LogMaintenanceWorker(IConfiguration cfg,
                                ILogger<LogMaintenanceWorker> log)
    {
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
                CompressOldTxtLogs();
                PurgeOldArchives();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Log maintenance failed");
            }

            await Task.Delay(TimeSpan.FromHours(24), stop);
        }
    }

    private void CompressOldTxtLogs()
    {
        if (string.IsNullOrWhiteSpace(_sevenZipExe) || !File.Exists(_sevenZipExe))
        {
            _log.LogWarning("SevenZipExe not configured or not found. Skipping compression step.");
            return;
        }

        var today = DateTime.Now.Date;

        foreach (var txtPath in Directory.EnumerateFiles(_logDir, "*.txt"))
        {
            var fileName = Path.GetFileNameWithoutExtension(txtPath);

            if (!DateTime.TryParseExact(fileName, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fileDate))
            {
                _log.LogWarning("Invalid log filename: {File}. Deleting.", txtPath);
                File.Delete(txtPath);
                continue;
            }

            // Skip today's log — it might still be written to
            if (fileDate >= today)
                continue;

            var sevenZipPath = Path.Combine(_logDir, $"{fileName}.7z");
            if (File.Exists(sevenZipPath))
            {
                _log.LogInformation("Archive already exists for {Date}, skipping compression.", fileName);
                continue;
            }

            var psi = new ProcessStartInfo
            {
                FileName = _sevenZipExe,
                Arguments = $"a -bb0 -t7z \"{sevenZipPath}\" \"{txtPath}\" -mx=9",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            using var proc = Process.Start(psi)!;
            proc.WaitForExit();

            if (proc.ExitCode != 0)
            {
                _log.LogError("7-Zip failed for {File}. Code: {Code}, Error: {Err}", txtPath, proc.ExitCode, proc.StandardError.ReadToEnd());
                continue;
            }

            File.Delete(txtPath);
            _log.LogInformation("Compressed and deleted log: {Txt} → {Seven}", txtPath, sevenZipPath);
        }
    }

    private void PurgeOldArchives()
    {
        var cutoff = DateTime.Now.AddDays(-_daysToKeep).Date;

        foreach (var sevenZipPath in Directory.EnumerateFiles(_logDir, "*.7z"))
        {
            var fileName = Path.GetFileNameWithoutExtension(sevenZipPath);

            if (!DateTime.TryParseExact(fileName, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fileDate))
            {
                _log.LogWarning("Delete unknown archive: {File}", sevenZipPath);
                File.Delete(sevenZipPath);
                continue;
            }

            if (fileDate < cutoff)
            {
                File.Delete(sevenZipPath);
                _log.LogInformation("Deleted expired archive: {File}", sevenZipPath);
            }
        }
    }
}
