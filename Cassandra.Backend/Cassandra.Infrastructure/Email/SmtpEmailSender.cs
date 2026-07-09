using MailKit.Net.Smtp;
using MailKit.Security;
using Cassandra.Application.Contracts.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Cassandra.Infrastructure.Email;

/// <summary>
/// SMTP email sender (MailKit). Reads the <c>Email</c> configuration section directly,
/// matching the project's configuration style (see JwtTokenService). In development this
/// points at the Mailpit container wired up by the Aspire AppHost; in production it points
/// at a real SMTP provider whose credentials come from configuration / environment.
/// </summary>
public class SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    public async Task SendAsync(
        string toEmail, string? toName, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        var section = configuration.GetSection("Email");
        var host = section["Host"] ?? "localhost";
        var port = int.TryParse(section["Port"], out var p) ? p : 1025;
        var useStartTls = bool.TryParse(section["UseStartTls"], out var t) && t;
        var user = section["User"];
        var password = section["Password"];
        var fromAddress = section["FromAddress"] ?? "no-reply@cassandra.local";
        var fromName = section["FromName"] ?? "Cassandra";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromAddress));
        message.To.Add(new MailboxAddress(toName ?? toEmail, toEmail));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();

        // The server certificate is still fully validated against the system trust store; we only
        // turn OFF the revocation (CRL/OCSP) check. MailKit enables revocation checking by default,
        // and in containers the CRL distribution points are frequently unreachable — which aborts
        // the handshake with "unable to get certificate CRL" even though Brevo's certificate is
        // otherwise valid. Disabling revocation is the standard remedy here and is NOT the same as
        // trusting any certificate (which would defeat TLS).
        client.CheckCertificateRevocation = false;

        // StartTls for real providers (587); None for the local Mailpit catcher (1025).
        var socketOptions = useStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
        await client.ConnectAsync(host, port, socketOptions, cancellationToken);

        if (!string.IsNullOrWhiteSpace(user))
            await client.AuthenticateAsync(user, password, cancellationToken);

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);

        logger.LogInformation("Sent email '{Subject}' to {Email} via {Host}:{Port}", subject, toEmail, host, port);
    }
}
