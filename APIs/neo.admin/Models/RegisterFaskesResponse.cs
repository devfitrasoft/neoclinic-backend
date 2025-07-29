using Shared.Models;

namespace neo.admin.Models
{
    public sealed class RegisterFaskesResponse : CommonAPIBodyResponse
    {
        public IEnumerable<RegisterFaskesResponseData> Data { get ; set; } = new List<RegisterFaskesResponseData>();
    }

    public sealed record RegisterFaskesResponseData(
        bool isRegistered,
        bool preExisted
    );
}
