using neo.preregist.Common;
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
            var otpExpiry = _cfg["PreRegistToken:Expiry"] ?? LocalConstants.OTP_EXPIRY_IN_MINUTE;

            var html = TemplateRenderer.Render("""
            <p>Terimakasih sudah memilih produk web dari aplikasi Neoclinic!</p>
            <p>Anda dapat melakukan registrasi faskes dengan meng-klik <a href="{{ link }}" target="_blank">link</a> berikut ini.</p>
            <br/>
            <p>Link registrasi hanya dapat berlaku selama {{ otpExpiry }} menit.</p>
            """, new { link, otpExpiry });

            await _email.SendAsync(toEmail,
                header,
                html);
        }
    }
}
