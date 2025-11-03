namespace Notification.Services;

public interface ITemplateService
{
    string GetEmailConfirmationTemplate(string fullName, string email, string confirmationUrl);
    string GetPasswordResetTemplate(string fullName, string email, string resetCode);
}
