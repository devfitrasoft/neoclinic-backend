using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace neo.admin.Models
{
    public sealed record ActivateFaskesRequest
    {
        [JsonPropertyName("username"), Required]
        public string LoginUsername { get; init; } = null!;
    }
}
