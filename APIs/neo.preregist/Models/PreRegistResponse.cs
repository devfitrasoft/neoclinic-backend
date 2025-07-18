using neo.preregist.Common;
using Shared.Common.Models;
using System.Text.Json.Serialization;

namespace neo.preregist.Models
{
    public class PreRegistResponse : CommonAPIBodyResponse
    {
        [JsonPropertyName("data")]
        public PreRegistResponseData Data { get; set; } = null!;
    }

    public sealed record PreRegistResponseData(
        PreRegistSaveResponse status,
        bool isRegistered
    );
}
