using Shared.Models;
using System.Text.Json.Serialization;

namespace neo.admin.Models
{
    public sealed class FaskesInfoResponseModel : CommonAPIBodyResponseModel
    {
        [JsonPropertyName("data")]
        public IEnumerable<FaskesInfoResponseDataModel> Data { get; set; } = new List<FaskesInfoResponseDataModel>();
    }

    public sealed record FaskesInfoResponseDataModel(
    long Id,
    string Name,
    string Email,
    string Phone,
    string Address,
    bool IsActive,
    long? CorporateId,
    string? CorporateName);
}
