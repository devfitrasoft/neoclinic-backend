using Shared.Communication.Http;

public interface IDBManagementClient
{
    Task<string> CreateClinicDatabaseAsync(long faskesId, CancellationToken ct);
}

public sealed class DBManagementClient : IDBManagementClient
{
    private readonly ILogger<DBManagementClient> _log;
    private readonly IRestClient _rest;
    private readonly string _serviceUrl;   // ← keep the base once
    public DBManagementClient(IRestClient rest, IConfiguration cfg, ILogger<DBManagementClient> log)
    {
        _log = log;
        _rest = rest;
        _serviceUrl = cfg["Services:DbManagement"] ?? "http://neo.db.mgmt";
    }

    public async Task<string> CreateClinicDatabaseAsync(long faskesId, CancellationToken ct)
    {
        var result = await _rest.PostAsync(
            $"{_serviceUrl}/clinic-databases",
            new { ClinicId = faskesId },      // anonymous request object
            ct);

        if (!result.IsSuccess)
            _log.LogError($"DB creation failed: {result.Error}");

        // The service follows the naming convention clinic_<id:00000000>
        return $"clinic_{faskesId:D8}";
    }
}
