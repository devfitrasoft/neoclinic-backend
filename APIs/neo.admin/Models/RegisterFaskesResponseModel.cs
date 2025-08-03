using Shared.Models;

namespace neo.admin.Models
{
    public sealed class RegisterFaskesResponseModel : CommonAPIBodyResponseModel
    {
        public IEnumerable<RegisterFaskesResponseDataModel> Data { get ; set; } = new List<RegisterFaskesResponseDataModel>();
    }

    public sealed record RegisterFaskesResponseDataModel(
        bool isRegistered,
        bool preExisted
    );
}
