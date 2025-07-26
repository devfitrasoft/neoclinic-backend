using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.Models
{
    public sealed record RegisterFaskesRequest
    {
        [JsonPropertyName("noFaskes"), Required]
        public string NoFaskes { get; init; } = null!;

        [JsonPropertyName("name"), Required]
        public string Name { get; init; } = null!;

        [JsonPropertyName("email")] public string? Email { get; init; }
        
        [JsonPropertyName("phone"), Required]
        public string Phone { get; init; } = string.Empty!;

        [JsonPropertyName("npwp"), Required]
        public string Npwp { get; init; } = string.Empty!;

        [JsonPropertyName("address"), Required]
        public string Address { get; init; } = null!;

        [JsonPropertyName("isCorporate"), Required]
        public bool IsCorporate { get; init; }

        [JsonPropertyName("corporateName")]
        public string? CorporateName { get; init; }

        [JsonPropertyName("corporateId")]
        public long? CorporateId { get; init; }

        [JsonPropertyName("namePj"), Required]
        public string NamePj { get; init; } = null!;

        [JsonPropertyName("emailPj"), Required]
        public string EmailPj { get; init; } = null!;

        [JsonPropertyName("phonePj"), Required]
        public string PhonePj { get; init; } = null!;

        [JsonPropertyName("nameBill"), Required]
        public string NameBill { get; init; } = null!;

        [JsonPropertyName("emailBill")]
        public string EmailBill { get; init; } = null!;

        [JsonPropertyName("phoneBill")]
        public string PhoneBill { get; init; } = null!;

        [JsonPropertyName("nameTech"), Required]
        public string NameTech { get; init; } = null!;

        [JsonPropertyName("emailTech")]
        public string EmailTech { get; init; } = null!;

        [JsonPropertyName("phoneTech")]
        public string PhoneTech { get; init; } = null!;

        [JsonPropertyName("captcha"), Required]
        public string CaptchaToken { get; init; } = null!;

        [JsonPropertyName("otp"), Required]
        public string Otp { get; init; } = null!;
    }
}
