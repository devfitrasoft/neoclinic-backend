using neo.preregist.Common;
using Shared.Common;
using Shared.Mailing;

namespace neo.preregist.Services
{
    public class MailService
    {
        private readonly IEmailSender _email;
        private readonly IConfiguration _cfg;

        public MailService( IConfiguration cfg,IEmailSender email)
        {
            _cfg = cfg;
            _email = email;
        }

        public async Task SendNotifAsync(string toEmail, string otp)
        {
            var header = LocalConstants.MAIL_REGIST_TOKEN_HEADER;

            var link = $"{_cfg["App:RegisterWebUrl"]}/register?token={otp}";
            var otpExpiry = _cfg["OtpToken:Expiry"] ?? Constants.OTP_EXPIRY_IN_MINUTE;

            var html = TemplateRenderer.Render("""
            <p>Terimakasih telah memilih NeoClinic!</p>
            <p>Silakan klik <a href="{{ link }}" target="_blank">link</a> untuk melengkapi data faskes Anda.</p>
            <br/>
            <p>Link hanya berlaku untuk {{ otpExpiry }} menit.</p>
            """, new { link, otpExpiry });

            await _email.SendAsync(toEmail,
                header,
                html);
        }
    }
}
