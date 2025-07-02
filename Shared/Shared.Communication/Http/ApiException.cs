using System;

namespace Shared.Communication.Http
{
    /// <summary>
    /// Thrown whenever the HTTP layer returns a non-success status code.
    /// </summary>
    public sealed class ApiException : Exception
    {
        /// <summary>
        /// HTTP status code returned by the remote server.
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="ApiException"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="payload">The raw response payload returned by the server (can be <c>null</c>).</param>
        public ApiException(int statusCode, string? payload)
            : base($"API error {statusCode}: {payload}") => StatusCode = statusCode;
    }
}
