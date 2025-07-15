using neo.admin.Common;
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

        public Task SendNotifAsync(string toEmail, ProductTypes product, string otp)
        {
            var header = determineHeader(product);
            var html = generateBody(product, otp);

            return _email.SendAsync(toEmail,
                header,
                html);
        }

        private string determineHeader(ProductTypes product)
            => product == ProductTypes.Web
            ? "Pemberitahuan Link Token Registrasi NeoClinic - Web"
            : product == ProductTypes.Desktop
                ? "Pemberitahuan Pra-Registrasi Neoclinic - Desktop"
                : "Pemberitahuan Pra-Registrasi Neoclinic";

        private string generateBody(ProductTypes product, string? otp)
        {
            var body = string.Empty;

            if(product == ProductTypes.Web)
            {
                var link = $"{_cfg["App:RegisterWebUrl"]}/registration?token={otp}";
                var otpExpiry = _cfg["PreRegistToken:Expiry"];

                body = TemplateRenderer.Render("""
                <p>Terimakasih sudah memilih produk Desktop dari aplikasi Neoclinic!</p>
                <p>Anda dapat melakukan registrasi faskes dengan meng-klik <a href="{{ link }}" target="_blank">link</a> berikut ini.</p>
                <br/>
                <p>Link registrasi hanya dapat berlaku selama {{ otpExpiry }} menit.</p>
                """, new { link, otpExpiry });
            }
            else if(product == ProductTypes.Desktop)
            {
                body = TemplateRenderer.Render("""
                <h2>Selamat datang di NeoClinic!</h2>
                <p>Terimakasih sudah memilih produk Desktop dari aplikasi Neoclinic!</p>
                <p>Anda akan dihubungi lebih lanjut oleh admin melalui email/whatsapp yang telah anda cantumkan sebelumnya.</p>
                """, new { });
            }

            return body;
        }
    }
}
