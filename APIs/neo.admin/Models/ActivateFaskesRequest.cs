using System.Text.Json.Serialization;

namespace neo.admin.Models
{
    public sealed record ActivateFaskesRequest
    {
        [JsonPropertyName("username")]
        public string LoginUsername { get; init; } = null!;
    }
}
