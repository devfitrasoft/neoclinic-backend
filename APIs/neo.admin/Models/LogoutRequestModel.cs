using System.Text.Json.Serialization;

namespace neo.admin.Models
{
    public class LogoutRequest
    {
        public string RefreshToken { get; set; } = null!;
        public long LoginId { get; set; }
    }

    public class LogoutRequestMobileModel : LogoutRequest
    {
        [JsonPropertyName("deviceId")]
        public string DeviceId { get; set; } = null!;

        [JsonPropertyName("userAgent")]
        public string UserAgent { get; set; } = null!;
    }

}
