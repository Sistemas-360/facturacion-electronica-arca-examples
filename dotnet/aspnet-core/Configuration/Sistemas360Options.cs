namespace Sistemas360Example.Api.Configuration;

public sealed class Sistemas360Options
{
    public const string SectionName = "Sistemas360";

    public string BaseUrl { get; set; } =
        "https://api.sistemas360.ar";

    public string Token { get; set; } = string.Empty;
}
