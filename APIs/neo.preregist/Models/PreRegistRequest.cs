using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace neo.preregist.Models
{
    public sealed record PreRegistRequest
    {
        [JsonPropertyName("name"), Required]
        public string Name { get; set; } = null!;

        [JsonPropertyName("preferred_comm"), Required]
        public int PrefComm { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("phone")]
        public string Phone { get; set; } = null!;

        [JsonPropertyName("product"), Required]
        public int ProductType { get; set; }
    }
}
