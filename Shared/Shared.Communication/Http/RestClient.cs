using Microsoft.Extensions.Logging;
using Shared.Communication.Results;
using System.Net.Http.Json;

namespace Shared.Communication.Http
{
    /// <summary>
    /// Default implementation that uses <see cref="HttpClient"/> with Polly retry logic.
    /// </summary>
    internal sealed class RestClient : IRestClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<RestClient> _log;

        public RestClient(HttpClient http, ILogger<RestClient> log)
        {
            _http = http;
            _log = log;
        }

        /// <inheritdoc />
        public Task<T?> GetAsync<T>(string url, CancellationToken ct = default) =>
            SendAsync<T>(HttpMethod.Get, url, null, ct);

        /// <inheritdoc />
        public Task<Result> PostAsync<TReq>(string url, TReq body, CancellationToken ct = default) =>
            SendAsync(HttpMethod.Post, url, body, ct);

        /// <inheritdoc />
        public Task<Result> PutAsync<TReq>(string url, TReq body, CancellationToken ct = default) =>
            SendAsync(HttpMethod.Put, url, body, ct);

        /// <inheritdoc />
        public Task<Result> DeleteAsync(string url, CancellationToken ct = default) =>
            SendAsync(HttpMethod.Delete, url, null, ct);

        /* ---------- private helpers ---------- */

        private async Task<Result> SendAsync(HttpMethod method, string url, object? body, CancellationToken ct)
        {
            await SendAsync<object>(method, url, body, ct).ConfigureAwait(false);
            return Result.Ok();
        }

        private async Task<T?> SendAsync<T>(HttpMethod method, string url, object? body, CancellationToken ct)
        {
            using var request = new HttpRequestMessage(method, url);

            if (body != null)
                request.Content = JsonContent.Create(body);

            var response = await _http.SendAsync(request, ct).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                string payload = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                _log.LogWarning("{Method} {Url} failed: {Status}", method, url, (int)response.StatusCode);
                throw new ApiException((int)response.StatusCode, payload);
            }

            // Short-circuit for void calls
            if (typeof(T) == typeof(object) || response.Content.Headers.ContentLength == 0)
                return default;

            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct).ConfigureAwait(false);
        }
    }
}
