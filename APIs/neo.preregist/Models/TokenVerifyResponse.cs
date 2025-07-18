using Shared.Common.Models;
using System.Text.Json.Serialization;

namespace neo.preregist.Models
{
    public sealed class TokenVerifyResponse : CommonAPIBodyResponse
    {
        [JsonPropertyName("data")]
        public PreRegistData? Data { get; set; }
    }

    public sealed record PreRegistData(
        string name,
        string email,
        string phone,
        DateTime? otpExpiresAt,
        bool isRegistered
    );
}
