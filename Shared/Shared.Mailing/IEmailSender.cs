using System.Threading.Tasks;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace Shared.Mailing;

/// <summary>Abstraction over any e‑mail transport.</summary>
public interface IEmailSender
{

    // Minimal case: just HTML
    Task SendAsync(
        string fromName,
        string fromAddress,
        string toAddress,
        string subject,
        string htmlBody,
        CancellationToken ct = default);

    Task SendAsync(
        string fromName,
        string fromAddress,
        IEnumerable<string> toAddresses,
        string subject,
        string htmlBody,
        CancellationToken ct = default);

    // HTML + Text fallback
    Task SendAsync(
        string fromName,
        string fromAddress,
        string toAddress,
        string subject,
        string htmlBody,
        string? textBody,
        CancellationToken ct = default);

    Task SendAsync(
        string fromName,
        string fromAddress,
        IEnumerable<string> toAddresses,
        string subject,
        string htmlBody,
        string textBody,
        CancellationToken ct = default);

    // Full advanced version (master)
    Task SendAsync(
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
        CancellationToken ct = default);

    Task SendAsync(
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
        CancellationToken ct = default);
}
