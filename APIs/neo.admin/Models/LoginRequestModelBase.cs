using System.Text.Json.Serialization;

namespace neo.admin.Models
{
    public class LoginRequestModelBase 
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = null!;
        [JsonPropertyName("password")]
        public string Password { get; set; } = null!;
    }

    public class LoginRequestWebModel : LoginRequestModelBase
    {
        [JsonPropertyName("captcha")]
        public string Captcha { get; set; } = null!;
    }
}
