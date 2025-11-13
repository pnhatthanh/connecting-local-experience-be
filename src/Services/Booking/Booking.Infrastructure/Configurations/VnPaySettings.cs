namespace Booking.Infrastructure.Configurations
{
    public class VnPaySettings
    {
        public const string VnPaySettingsKey = "VnPaySettings";
        
        public string TmnCode { get; set; } = string.Empty;
        public string HashSecret { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string Version { get; set; } = "2.1.0";
    }
}
