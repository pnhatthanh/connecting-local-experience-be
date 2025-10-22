namespace Notification.Services;

public class TemplateService : ITemplateService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<TemplateService> _logger;

    public TemplateService(IWebHostEnvironment environment, ILogger<TemplateService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public string GetEmailConfirmationTemplate(string fullName, string confirmationUrl)
    {
        var template = LoadTemplate("EmailConfirmation.html");
        return template
            .Replace("{{FullName}}", fullName)
            .Replace("{{ConfirmationUrl}}", confirmationUrl);
    }

    public string GetPasswordResetTemplate(string fullName, string resetUrl)
    {
        var template = LoadTemplate("PasswordReset.html");
        return template
            .Replace("{{FullName}}", fullName)
            .Replace("{{ResetUrl}}", resetUrl);
    }

    private string LoadTemplate(string templateName)
    {
        try
        {
            var templatePath = Path.Combine(_environment.ContentRootPath, "Templates", templateName);
            
            if (!File.Exists(templatePath))
            {
                _logger.LogWarning("Template file not found: {TemplatePath}", templatePath);
                return GetFallbackTemplate(templateName);
            }

            return File.ReadAllText(templatePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading template: {TemplateName}", templateName);
            return GetFallbackTemplate(templateName);
        }
    }

    private string GetFallbackTemplate(string templateName)
    {
        // Fallback simple template if file not found
        return templateName switch
        {
            "EmailConfirmation.html" => "<html><body><h1>Email Confirmation</h1><p>Hello {{FullName}},</p><p>Please confirm your email: <a href='{{ConfirmationUrl}}'>Click here</a></p></body></html>",
            "PasswordReset.html" => "<html><body><h1>Password Reset</h1><p>Hello {{FullName}},</p><p>Reset your password: <a href='{{ResetUrl}}'>Click here</a></p></body></html>",
            _ => "<html><body><p>Email template</p></body></html>"
        };
    }
}
