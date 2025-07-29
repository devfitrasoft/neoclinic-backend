using System.Text.Json.Serialization;

namespace Shared.Models
{
    public class CommonAPIBodyResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName ("message")]
        public string Message { get; set; } = string.Empty;
    }
}
