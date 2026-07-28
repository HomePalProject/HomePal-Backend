using HomePal.Application.Common.Interfaces;
using HomePal.Infrastructure.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Globalization;

namespace HomePal.Infrastructure.Email;

public class MailKitEmailSender : IEmailSender
{
    private readonly MailKitOptions _options;
    private readonly ILogger<MailKitEmailSender> _logger;

    public MailKitEmailSender(IOptions<MailKitOptions> options, ILogger<MailKitEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            var secureOption = _options.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;

            await client.ConnectAsync(_options.Host, _options.Port, secureOption, cancellationToken);
            if (!string.IsNullOrWhiteSpace(_options.Username))
            {
                await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email successfully sent to {ToEmail} with subject {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {ToEmail} with subject {Subject}", toEmail, subject);
            throw;
        }
    }

    public async Task SendConfirmationEmailAsync(string toEmail, string fullName, string confirmationLink, CancellationToken cancellationToken = default)
    {
        var template = await GetTemplateAsync("ConfirmEmail.html", cancellationToken);
        var body = template
            .Replace("{{Name}}", fullName)
            .Replace("{{ConfirmationLink}}", confirmationLink);

        await SendEmailAsync(toEmail, "Confirm your HomePal email", body, cancellationToken);
    }

    public async Task SendResetPasswordEmailAsync(string toEmail, string fullName, string resetLink, CancellationToken cancellationToken = default)
    {
        var template = await GetTemplateAsync("ResetPassword.html", cancellationToken);
        var body = template
            .Replace("{{Name}}", fullName)
            .Replace("{{ResetLink}}", resetLink);

        await SendEmailAsync(toEmail, "Reset your HomePal password", body, cancellationToken);
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string fullName, CancellationToken cancellationToken = default)
    {
        var template = await GetTemplateAsync("Welcome.html", cancellationToken);
        var body = template
            .Replace("{{Name}}", fullName);

        await SendEmailAsync(toEmail, "Welcome to HomePal!", body, cancellationToken);
    }

    public async Task SendInvitationEmailAsync(string toEmail, string inviterName, CancellationToken cancellationToken = default)
    {
        var template = await GetTemplateAsync("Invitation.html", cancellationToken);
        var body = template
            .Replace("{{InviterName}}", inviterName);

        await SendEmailAsync(toEmail, "Invitation to join HomePal Household", body, cancellationToken);
    }

    private static async Task<string> GetTemplateAsync(string templateName, CancellationToken cancellationToken)
    {
        var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();

        if (culture != "en")
        {
            var localizedName = templateName.Replace(".html", $".{culture}.html");
            var localizedPath = Path.Combine(AppContext.BaseDirectory, "Templates", localizedName);
            if (File.Exists(localizedPath))
            {
                return await File.ReadAllTextAsync(localizedPath, cancellationToken);
            }
        }

        var defaultPath = Path.Combine(AppContext.BaseDirectory, "Templates", templateName);
        return await File.ReadAllTextAsync(defaultPath, cancellationToken);
    }
}
