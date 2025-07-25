﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace neo.admin.Models
{
    public sealed record RegisterFaskesRequest
    {
        [JsonPropertyName("noFaskes"), Required]
        public string NoFaskes { get; init; } = null!;

        [JsonPropertyName("name"), Required]
        public string Name { get; init; } = null!;

        [JsonPropertyName("address"), Required]
        public string Address { get; init; } = null!;

        [JsonPropertyName("isCorporate"), Required]
        public bool IsCorporate { get; init; }

        [JsonPropertyName("corporateName"), Required]
        public string? CorporateName { get; init; }

        [JsonPropertyName("corporateId"), Required]
        public long? CorporateId { get; init; }

        [JsonPropertyName("email"), Required]
        public string Email { get; init; } = null!;

        [JsonPropertyName("phone"), Required]
        public string Phone { get; init; } = null!;

        [JsonPropertyName("captcha"), Required]
        public string CaptchaToken { get; init; } = null!;
    }
}
