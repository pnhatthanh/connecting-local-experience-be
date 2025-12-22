namespace Experience.Infrastructure.Configurations
{
    public class CloudinarySettings
    {
        public const string CloudinarySettingsKey = "Cloudinary";
        
        public string CloudName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
        public string DefaultFolder { get; set; } = "experiences";
    }
}
