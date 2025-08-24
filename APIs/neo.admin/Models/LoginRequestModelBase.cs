using System.Text.Json.Serialization;

namespace neo.admin.Models
{
    public interface ILoginRequestModel { }
    public class LoginRequestModelBase : ILoginRequestModel
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = null!;
        
        [JsonPropertyName("password")]
        public string Password { get; set; } = null!;

        [JsonPropertyName("tanggalJaga")]
        public DateTime TanggalJaga { get; set; }

        [JsonPropertyName("shift")]
        public int Shift { get; set; }
    }

    public class LoginRequestWebModel : LoginRequestModelBase
    {
        [JsonPropertyName("captcha")]
        public string Captcha { get; set; } = null!;
    }

    public class LoginRequestMobileModel : LoginRequestModelBase {}

    public class LoginSessionContext
    {
        public DateTime? TanggalJaga { get; set; }
        public int? Shift { get; set; }
    }
}
