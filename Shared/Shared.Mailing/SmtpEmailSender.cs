using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Threading;

namespace Shared.Mailing;

/// <summary>Concrete SMTP implementation of <see cref="IEmailSender"/>.</summary>
internal sealed class SmtpEmailSender : IEmailSender
{
    private readonly SmtpSettings _settings;

    public SmtpEmailSender(IOptions<SmtpSettings> options) =>
        _settings = options.Value;

    public async Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken ct = default)
    {
        var msg = new MimeMessage
        {
            Subject = subject
        };

        msg.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
        msg.To.Add(MailboxAddress.Parse(to));

        msg.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(
            _settings.Host,
            _settings.Port,
            _settings.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto,
            ct);

        await client.AuthenticateAsync(_settings.Username, _settings.Password, ct);
        await client.SendAsync(msg, ct);
        await client.DisconnectAsync(true, ct);
    }
}
