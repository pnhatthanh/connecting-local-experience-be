namespace Notification.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody);
    Task SendEmailConfirmationAsync(string to, string fullName, string confirmationUrl);
    Task SendPasswordResetAsync(string to, string fullName, string resetUrl);
}
