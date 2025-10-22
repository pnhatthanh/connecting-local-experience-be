namespace Notification.Services;

public interface ITemplateService
{
    string GetEmailConfirmationTemplate(string fullName, string confirmationUrl);
    string GetPasswordResetTemplate(string fullName, string resetUrl);
}
