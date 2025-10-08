namespace IAM.Infrastructure.Configurations
{
    public class JwtSetting
    {
        public static string JwtSettingKey = "Jwt";
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public double RefreshTokenExpirationDays { get; set; }
        public double ExpirationMinutes { get; set; }
    }
}