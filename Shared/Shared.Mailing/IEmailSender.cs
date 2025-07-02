using System.Threading.Tasks;
using System.Threading;

namespace Shared.Mailing
{
    /// <summary>Abstraction over any e‑mail transport.</summary>
    interface IEmailSender
    {
        Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken ct = default);
    }
}
