using Microsoft.Extensions.Options;
using neo.admin.Models;
using Shared.Mailing;

namespace neo.admin.Services
{
    public class MailService
    {
        private readonly IEmailSender _email;
        private readonly IConfiguration _cfg;
        private readonly RegistrationSettings _regSettings;

        public MailService( IConfiguration cfg, IEmailSender email, IOptions<RegistrationSettings> regSettings)
        {
            _cfg = cfg;
            _email = email;
            _regSettings = regSettings.Value;
        }

        public Task SendInviteAsync(string toEmail, string loginUsername, Tuple<string,DateTime> token)
        {
            var link = $"{_cfg["App:RegisterWebUrl"]}/reset-password?token={token.Item1}";
            var safeUsername = loginUsername.Replace(".", "&#8203;.");
            var html = TemplateRenderer.Render("""
            <h2>Selamat datang di NeoClinic</h2>
            <p>Akun Super User Anda (<b>{{ loginUsername }}</b>) hampir siap. Klik tautan di bawah untuk membuat kata sandi:</p>
            <p><a href="{{ link }}">Setel Kata Sandi</a></p>
            """, new { loginUsername = safeUsername, link });

            return _email.SendAsync(toEmail,
                "Aktivasi akun NeoClinic",
                html);
        }

        public Task SendRegistrationFeeAsync(string toEmail, string faskesName)
        {
            decimal fee = _regSettings.Fee;
            string rekening = _regSettings.BankAccountNumber;
            string phone = _regSettings.ConfirmPaymentPhoneNumber;

            var html = TemplateRenderer.Render("""
            <h2>Registrasi Hampir Selesai!</h2>
            <p>Silahkan lakukan pembayaran sebesar Rp.{{ fee }} melalui rekening {{ rekening }}</p>
            <p>dan konfirmasikan melalui WhatsApp di nomor {{ phone }}.</p>
            """, new { fee, rekening, phone });

            return _email.SendAsync(toEmail,
                $"Konfirmasi Pembayaran akun NeoClinic - {faskesName}",
                html);
        }
    }
}
