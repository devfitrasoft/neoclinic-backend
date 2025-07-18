using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace neo.preregist.Models
{
    public sealed record PreRegistRequest
    {
        [JsonPropertyName("name"), Required]
        public string Name { get; set; } = null!;

        [JsonPropertyName("email"), Required]
        public string Email { get; set; } = null!;

        [JsonPropertyName("phone"), Required]
        public string Phone { get; set; } = null!;
    }
}
