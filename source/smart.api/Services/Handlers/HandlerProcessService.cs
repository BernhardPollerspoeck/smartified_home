namespace smart.api.Services.Handlers;

public class HandlerProcessService : BackgroundService
{
    private readonly IConfiguration _configuration;

    public HandlerProcessService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //start all handler

        //read to start new handler

        var id = _configuration.GetValue<int>("id");
        var db = _configuration.GetValue<string>("database");

        //stop handlers

        return Task.CompletedTask;
    }
}
