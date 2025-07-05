using System.Text.Json.Serialization;

namespace neo.admin.Models
{
    public sealed record RegisterFaskesRequest
    {
        [JsonPropertyName("noFaskes")]
        public string NoFaskes { get; init; } = null!;

        [JsonPropertyName("name")]
        public string Name { get; init; } = null!;

        [JsonPropertyName("address")]
        public string Address { get; init; } = null!;

        [JsonPropertyName("isCorporate")]
        public bool IsCorporate { get; init; }

        [JsonPropertyName("corporateName")]
        public string? CorporateName { get; init; }

        [JsonPropertyName("corporateId")]
        public long? CorporateId { get; init; }

        [JsonPropertyName("email")]
        public string Email { get; init; } = null!;

        [JsonPropertyName("phone")]
        public string Phone { get; init; } = null!;

        [JsonPropertyName("captcha")]
        public string CaptchaToken { get; init; } = null!;
    }
}
