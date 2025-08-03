namespace neo.admin.Models
{
    public class RefreshTokenMobileRequestModel
    {
        public string RefreshToken { get; set; } = null!;
        public string DeviceId { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
    }
}
