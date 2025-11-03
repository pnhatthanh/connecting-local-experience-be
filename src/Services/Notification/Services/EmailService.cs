using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Notification.Configurations;

namespace Notification.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly ITemplateService _templateService;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<SmtpSettings> smtpSettings, 
        ITemplateService templateService,
        ILogger<EmailService> logger)
    {
        _smtpSettings = smtpSettings.Value;
        _templateService = templateService;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromAddress));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            
            await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, 
                _smtpSettings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            
            await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {To} with subject: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }

    public async Task SendEmailConfirmationAsync(string to, string fullName, string confirmationUrl)
    {
        var subject = "Confirm Your Email - Connecting Experience";
        var htmlBody = _templateService.GetEmailConfirmationTemplate(fullName, to, confirmationUrl);
        await SendEmailAsync(to, subject, htmlBody);
    }

    public async Task SendPasswordResetAsync(string to, string fullName, string resetCode)
    {
        var subject = "Reset Your Password - Connecting Experience";
        var htmlBody = _templateService.GetPasswordResetTemplate(fullName, to, resetCode);
        await SendEmailAsync(to, subject, htmlBody);
    }
}

