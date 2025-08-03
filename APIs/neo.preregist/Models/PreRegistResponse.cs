using neo.preregist.Common;
using Shared.Models;
using System.Text.Json.Serialization;

namespace neo.preregist.Models
{
    public class PreRegistResponse : CommonAPIBodyResponseModel
    {
        [JsonPropertyName("data")]
        public IEnumerable<PreRegistResponseData> Data { get; set; } = new List<PreRegistResponseData>();
    }

    public sealed record PreRegistResponseData(
        PreRegistSaveResponse status,
        bool isRegistered
    );
}
