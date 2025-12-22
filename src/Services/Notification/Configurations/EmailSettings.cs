namespace Notification.Configurations;

public class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = "Connecting Experience";
    public bool UseSsl { get; set; } = true;
}

public class EmailSettings
{
    public string VerificationUrlTemplate { get; set; } = string.Empty;
    public string ResetUrlTemplate { get; set; } = string.Empty;
}
