using Shared.Communication.Results;
using System.Threading.Tasks;
using System.Threading;

namespace Shared.Communication.Http
{
    /// <summary>
    /// Generic REST helper abstraction so callers can swap the implementation (for tests or auth-aware wrappers).
    /// </summary>
    public interface IRestClient
    {
        /// <summary>
        /// Sends an HTTP <c>GET</c> request and deserialises the JSON body into <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Expected response DTO type.</typeparam>
        /// <param name="url">Absolute or relative request URL.</param>
        /// <param name="cancellationToken">Optional token that can cancel the request.</param>
        /// <returns>The deserialised response, or <c>null</c> when the body is empty.</returns>
        Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an HTTP <c>POST</c> request with a JSON‑serialised body.
        /// </summary>
        /// <typeparam name="TRequest">Type of the request payload.</typeparam>
        /// <param name="url">Absolute or relative request URL.</param>
        /// <param name="body">Request object that will be serialised as JSON.</param>
        /// <param name="cancellationToken">Optional token that can cancel the request.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        Task<Result> PostAsync<TRequest>(string url, TRequest body, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an HTTP <c>PUT</c> request with a JSON‑serialised body.
        /// </summary>
        /// <typeparam name="TRequest">Type of the request payload.</typeparam>
        /// <param name="url">Absolute or relative request URL.</param>
        /// <param name="body">Request object that will be serialised as JSON.</param>
        /// <param name="cancellationToken">Optional token that can cancel the request.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        Task<Result> PutAsync<TRequest>(string url, TRequest body, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an HTTP <c>DELETE</c> request.
        /// </summary>
        /// <param name="url">Absolute or relative request URL.</param>
        /// <param name="cancellationToken">Optional token that can cancel the request.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        Task<Result> DeleteAsync(string url, CancellationToken cancellationToken = default);
    }

}
