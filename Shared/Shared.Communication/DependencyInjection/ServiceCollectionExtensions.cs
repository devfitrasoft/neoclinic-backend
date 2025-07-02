using Microsoft.Extensions.DependencyInjection;
using Polly;
using System.Net;

namespace Shared.Communication.DependencyInjection
{
    /// <summary>
    /// One-line DI helper to plug the REST client into any microservice.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the default <c>RestClient</c> and attaches a simple retry‑with‑back‑off policy for transient failures.
        /// </summary>
        /// <param name="services">The DI service collection.</param>
        /// <param name="clientName">Optional name used when configuring <see cref="HttpClient"/>.</param>
        public static IServiceCollection AddSharedRestClient(
            this IServiceCollection services,
            string clientName = "shared-rest")
        {
            services.AddHttpClient<Http.IRestClient, Http.RestClient>(clientName)
                    .AddPolicyHandler(GetRetryPolicy());
            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
        Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => r.StatusCode == HttpStatusCode.InternalServerError ||
                           r.StatusCode == HttpStatusCode.BadGateway ||
                           r.StatusCode == HttpStatusCode.GatewayTimeout ||
                           r.StatusCode == HttpStatusCode.ServiceUnavailable)
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(200 * attempt));
    }
}
