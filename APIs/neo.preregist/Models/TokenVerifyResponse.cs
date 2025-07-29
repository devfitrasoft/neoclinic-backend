using Shared.Models;
using System.Text.Json.Serialization;

namespace neo.preregist.Models
{
    public sealed class TokenVerifyResponse : CommonAPIBodyResponse
    {
        [JsonPropertyName("data")]
        public IEnumerable<PreRegistData> Data { get; set; } = new List<PreRegistData>();
    }

    public sealed record PreRegistData(
        string name,
        string email,
        string phone,
        DateTime? otpExpiresAt,
        bool isRegistered
    );
}
