namespace smart.core.Models;

public sealed class ApiSettings
{
    public string Database { get; init; } = default!;
    public string Secret { get; set; } = default!;
    public string Urls { get; init; } = default!;


}
