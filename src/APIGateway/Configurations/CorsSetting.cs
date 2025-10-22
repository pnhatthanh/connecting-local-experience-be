namespace APIGateway.Configurations;

public class CorsSetting
{
    public const string SectionName = "CorsSettings";
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}
