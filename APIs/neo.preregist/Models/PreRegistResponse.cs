using neo.admin.Common;
using Shared.Common.Models;
using System.Text.Json.Serialization;

namespace neo.preregist.Models
{
    public class PreRegistResponse : CommonAPIBodyResponse
    {
        [JsonPropertyName("status")]
        public PreRegistSaveResponse Status { get; set; }

        [JsonPropertyName("prefComm")]
        public PrefComms PrefComm { get; set; }

        [JsonPropertyName("isRegisteredWeb")]
        public bool IsRegisteredWeb { get; set; }

        [JsonPropertyName("isRegisteredDesktop")]
        public bool IsRegisteredDesktop { get; set; }
    }
}
