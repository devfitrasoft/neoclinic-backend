namespace Shared.Mailing
{
    /// <summary>Bound from configuration section "Smtp".</summary>
    public sealed class SmtpSettings
    {
        public string Host { get; set; } = default!;
        public int Port { get; set; } = 587;
        public bool UseStartTls { get; set; } = true;

        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;

        public string FromAddress { get; set; } = default!;
        public string FromName { get; set; } = "NeoClinic App";
    }
}
