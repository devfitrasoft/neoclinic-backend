namespace Shared.Communication.Results
{
    /// <summary>
    /// Lightweight success / failure wrapper that is compatible with .NET Standard 2.0 (records are not used).
    /// </summary>
    public struct Result
    {
        /// <summary>Gets a value indicating whether the operation succeeded.</summary>
        public bool IsSuccess { get; }

        /// <summary>Gets an error message when <see cref="IsSuccess"/> is <c>false</c>; otherwise <c>null</c>.</summary>
        public string? Error { get; }

        private Result(bool isSuccess, string? error = null)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        /// <summary>Create a successful result.</summary>
        public static Result Ok() => new Result(true);

        /// <summary>Create a failed result.</summary>
        public static Result Fail(string error) => new Result(false, error);
    }
}
