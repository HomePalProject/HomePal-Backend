namespace HomePal.Application.Common.Interfaces;

public interface IEmailSender
{
    Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default);
    Task SendConfirmationEmailAsync(string toEmail, string fullName, string confirmationLink, CancellationToken cancellationToken = default);
    Task SendResetPasswordEmailAsync(string toEmail, string fullName, string resetLink, CancellationToken cancellationToken = default);
    Task SendWelcomeEmailAsync(string toEmail, string fullName, CancellationToken cancellationToken = default);
    Task SendInvitationEmailAsync(string toEmail, string inviterName, CancellationToken cancellationToken = default);
}
