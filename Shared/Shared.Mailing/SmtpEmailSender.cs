using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Mailing;

/// <summary>Concrete SMTP implementation of <see cref="IEmailSender"/>.</summary>
public sealed class SmtpEmailSender : IEmailSender
{
    private readonly SmtpSettings _settings;
    private readonly Func<SmtpClient> _clientFactory;

    public SmtpEmailSender(IOptions<SmtpSettings> options, Func<SmtpClient>? clientFactory = null)
    {
        _settings = options.Value ?? throw new ArgumentNullException(nameof(options));
        _clientFactory = clientFactory ?? (() => new SmtpClient());
    }

    // --- Overload 1: minimal ---
    public Task SendAsync(
        string fromName,
        string fromAddress,
        string toAddress,
        string subject,
        string htmlBody,
        CancellationToken ct = default) =>
        SendAsync(fromName, fromAddress, new[] { toAddress }, subject, htmlBody, null, null, null, null, null, ct);

    public Task SendAsync(
        string fromName,
        string fromAddress,
        IEnumerable<string> toAddresses,
        string subject,
        string htmlBody,
        CancellationToken ct = default) =>
        SendAsync(fromName, fromAddress, toAddresses, subject, htmlBody, null, null, null, null, null, ct);

    // --- Overload 2: HTML + text ---
    public Task SendAsync(
        string fromName,
        string fromAddress,
        string toAddress,
        string subject,
        string htmlBody,
        string? textBody,
        CancellationToken ct = default) =>
        SendAsync(fromName, fromAddress, new[] { toAddress }, subject, htmlBody, textBody, null, null, null, null, ct);

    public Task SendAsync(
        string fromName,
        string fromAddress,
        IEnumerable<string> toAddresses,
        string subject,
        string htmlBody,
        string? textBody,
        CancellationToken ct = default) =>
        SendAsync(fromName, fromAddress, toAddresses, subject, htmlBody, textBody, null, null, null, null, ct);

    // --- Master implementation ---
    public Task SendAsync(
        string fromName,
        string fromAddress,
        string toAddress,
        string subject,
        string htmlBody,
        string? textBody = null,
        IEnumerable<string>? ccs = null,
        IEnumerable<string>? bccs = null,
        string? replyTo = null,
        IEnumerable<(string fileName, byte[] content, string mimeType)>? attachments = null,
        CancellationToken ct = default) =>
        SendAsync(fromName, fromAddress, new[] { toAddress }, subject, htmlBody, textBody, ccs, bccs, replyTo, attachments, ct);

    public async Task SendAsync(
        string fromName,
        string fromAddress,
        IEnumerable<string> toAddresses,
        string subject,
        string htmlBody,
        string? textBody = null,
        IEnumerable<string>? ccs = null,
        IEnumerable<string>? bccs = null,
        string? replyTo = null,
        IEnumerable<(string fileName, byte[] content, string mimeType)>? attachments = null,
        CancellationToken ct = default)
    {

        if (toAddresses == null || !toAddresses.Any())
            throw new ArgumentException("At least one recipient must be specified.", nameof(toAddresses));

        var msg = new MimeMessage
        {
            Subject = subject
        };

        if (!string.IsNullOrWhiteSpace(replyTo))
            msg.ReplyTo.Add(MailboxAddress.Parse(replyTo));

        msg.From.Add(new MailboxAddress(fromName, fromAddress));
        AddAddresses(msg.To, toAddresses);

        if (ccs != null) AddAddresses(msg.Cc, ccs);
        if (bccs != null) AddAddresses(msg.Bcc, bccs);

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlBody ?? string.Empty
        };

        if (!string.IsNullOrEmpty(textBody))
            bodyBuilder.TextBody = textBody;

        if (attachments != null)
        {
            foreach (var (fileName, content, mimeType) in attachments)
            {
                if (content == null || content.Length == 0) continue;

                var stream = new MemoryStream(content);
                bodyBuilder.Attachments.Add(fileName, stream, ContentType.Parse(mimeType));
            }
        }

        msg.Body = bodyBuilder.ToMessageBody();

        using var client = _clientFactory();

        await client.ConnectAsync(
            _settings.Host,
            _settings.Port,
            _settings.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto,
            ct);

        // Some servers allow no-auth; only authenticate if provided.
        if (!string.IsNullOrWhiteSpace(_settings.Username))
            await client.AuthenticateAsync(_settings.Username, _settings.Password, ct);

        await client.SendAsync(msg, ct);
        await client.DisconnectAsync(true, ct);
    }

    private static void AddAddresses(InternetAddressList list, IEnumerable<string> addresses)
    {
        foreach (var a in addresses)
        {
            if (string.IsNullOrWhiteSpace(a)) continue;
            list.Add(MailboxAddress.Parse(a));
        }
    }
}
