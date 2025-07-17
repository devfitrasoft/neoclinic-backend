using neo.preregist.Data.Enterprise.Entities;
using Shared.Common.Models;
using System.Text.Json.Serialization;

namespace neo.preregist.Models
{
    public sealed class TokenVerifyResponse : CommonAPIBodyResponse
    {
        [JsonPropertyName("data")]
        public PreRegist? Data { get; set; }
    }
}
