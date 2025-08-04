using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.Models
{
    public sealed class ResetPasswordReq
    {
        [Required, JsonPropertyName("password")] public string Password { get; set; } = null!;
        [Required, JsonPropertyName("rePassword")] public string RePassword { get; set; } = null!;
        [Required, JsonPropertyName("captcha")] public string Captcha { get; set; } = null!;
    }
}
