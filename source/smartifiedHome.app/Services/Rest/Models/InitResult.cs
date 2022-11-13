namespace smartifiedHome.app.Services.Rest.Models;
public class InitResult
{
    public bool Expired { get; init; }
    public bool Success { get; init; }
    public bool MissingRoute { get; set; }
}
