using Microsoft.Extensions.Options;
using smart.api.Models;
using smart.contract;
using smart.core.Models;
using smart.database;
using smart.resources;
using System.Diagnostics;
using System.Threading.Channels;

namespace smart.api.Services.Handlers;

public class HandlerProcessService : BackgroundService
{
    #region fields
    private readonly IOptions<ApiSettings> _options;
    private readonly ILogger<HandlerProcessService> _logger;
    private readonly SmartContext _context;
    private readonly ChannelReader<ElementHandler> _channelReader;
    private readonly List<HandlerProcessInfo> _runningProcesses;
    #endregion

    #region ctor
    public HandlerProcessService(
        IOptions<ApiSettings> options,
        ILogger<HandlerProcessService> logger,
        SmartContext context,
        ChannelReader<ElementHandler> channelReader)
    {
        _options = options;
        _logger = logger;
        _context = context;
        _channelReader = channelReader;
        _runningProcesses = new();
    }
    #endregion

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Handler Processing");
        #region startup - start all handlers in database
        foreach (var handler in _context.ElementHandlers)
        {
            await StartHandler(handler);
        }
        #endregion

        await foreach (var handler in _channelReader.ReadAllAsync(stoppingToken))
        {
            await StartHandler(handler);
        }

        #region shutdown = stop all handler processes
        _logger.LogInformation("Stopping all handlers");
        foreach (var process in _runningProcesses)
        {
            _logger.LogDebug("Stopping {Name}", process.Handler.Name);
            process.Process.CloseMainWindow();
            await Task.Delay(1000, CancellationToken.None);
            process.Process.Dispose();
        }
        #endregion
    }


    #region helper
    private static string? GetHandlerExecuteable(EHandlerType handlerType)
    {
        return handlerType switch
        {
            EHandlerType.Shelly => "smart.handler.shelly",

            _ => null,
        };
    }
    private async Task StartHandler(ElementHandler handler)
    {
        _logger.LogInformation("Starting {name}", handler.Name);
        var handlerExecuteable = GetHandlerExecuteable(handler.HandlerType);
        if (handlerExecuteable is null)
        {
            return;
        }
        var process = new Process();
        process.StartInfo.FileName = $"Handler/{handlerExecuteable}";
        process.StartInfo.Arguments = $"id={handler.Id} database={_options.Value.Database}";

        _logger.LogInformation("Creating Process for {name}", handler.Name);

        try
        {
            process.Start();
            _logger.LogInformation("Started {name}", handler.Name);
            _runningProcesses.Add(new HandlerProcessInfo(handler, process, handlerExecuteable));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not start {name}", handler.Name);
            _context.Log.Add(new LogItem
            {
                Type = $"{SmartResources.Handler}: {handler.HandlerType}",
                HandlerName = handler.Name,
                MetaInfo = ex.ToString(),
                Timestamp = DateTime.UtcNow,
            });
            await _context.SaveChangesAsync();
        }
    }
    #endregion
}
