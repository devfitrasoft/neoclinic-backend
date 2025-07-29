using Shared.Models;
using System.Text.Json.Serialization;

namespace neo.admin.Models
{
    public sealed class FaskesInfoResponse : CommonAPIBodyResponse
    {
        [JsonPropertyName("data")]
        public IEnumerable<FaskesInfoResponseData> Data { get; set; } = new List<FaskesInfoResponseData>();
    }

    public sealed record FaskesInfoResponseData(
    long Id,
    string Name,
    string Email,
    string Phone,
    string Address,
    bool IsActive,
    long? CorporateId,
    string? CorporateName);
}
