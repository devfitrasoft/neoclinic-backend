using neo.admin.Common;
using Shared.Common.Models;
using System.Text.Json.Serialization;

namespace neo.preregist.Models
{
    public class PreRegistResponse : CommonAPIBodyResponse
    {
        [JsonPropertyName("data")]
        public PreRegistResponseData Data { get; set; }
    }

    public sealed record PreRegistResponseData(
        PreRegistSaveResponse status,
        PrefComms prefComm,
        bool isRegisteredWeb,
        bool isRegisteredDesktop
        );
}
