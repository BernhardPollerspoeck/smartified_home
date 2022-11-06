using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using smart.contract;
using smart.contract.Handler;
using smart.core.Models;
using smart.database;
using smart.handler.shelly.Models;
using System.Globalization;
using System.Net.WebSockets;
using System.Text.Json;

namespace smart.handler.shelly.Services;

internal class MainHandler : BackgroundService
{
    #region fields
    private readonly List<HomeElement> _elements;

    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<HandlerSettings> _options;
    private readonly IHttpClientFactory _clientFactory;

    private HubConnection _hubConnection;
    #endregion

    #region ctor
    public MainHandler(
        IServiceProvider serviceProvider,
        IOptions<HandlerSettings> options,
        IHttpClientFactory clientFactory)
    {
        _elements = new();
        _serviceProvider = serviceProvider;
        _options = options;
        _clientFactory = clientFactory;
    }
    #endregion

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        #region hub connection
        _hubConnection = new HubConnectionBuilder()
        .WithUrl("http://localhost:5011/handlerHub")
            .Build();
        _hubConnection.Closed += async (error) =>
        {
            Console.WriteLine("Connection lost. retry in a bit");
            while (_hubConnection.State is not HubConnectionState.Connected)
            {
                await Task.Delay(3000);
                try
                {
                    var cts = new CancellationTokenSource();
                    cts.CancelAfter(2000);
                    Console.WriteLine("Trying connection");
                    await _hubConnection.StartAsync(cts.Token);
                }
                catch { }//continue

            }
            await _hubConnection.SendAsync("ReportHandlerType", _options.Value.Id, EHandlerType.Shelly1, cancellationToken: stoppingToken);

        };

        _hubConnection.On<int, string>("NewElement", HandleNewElement);
        _hubConnection.On<int, string>("SendElementCommand", HandleElementCommand);

        await _hubConnection.StartAsync(stoppingToken);

        await _hubConnection.SendAsync("ReportHandlerType", _options.Value.Id, EHandlerType.Shelly1, cancellationToken: stoppingToken);
        #endregion

        #region load my elements
        var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SmartContext>();

        var handler = db
            .ElementHandlers
            .Include(h => h.HomeElements)
            .First(h => h.Id == _options.Value.Id);
        foreach (var item in handler.HomeElements)
        {
            _elements.Add(item);
        }
        #endregion

        #region tmp
        while (!stoppingToken.IsCancellationRequested)
        {
            continue;
            foreach (var item in _elements)
            {
                var shellyState = await GetState(item.ConnectionInfo, item.Id);
                if (shellyState is not null)
                {
                    item.StateData = JsonSerializer.Serialize(shellyState);
                }
            }
            await db.SaveChangesAsync(stoppingToken);

            await Task.Delay(2000, stoppingToken);
        }
        #endregion
        //oe reconnect 
    }

    #region signalR handler
    private async Task HandleNewElement(int id, string connection)
    {
        var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SmartContext>();

        var newElement = await db.Elements.FirstOrDefaultAsync(e => e.Id == id);
        if (newElement is not null)
        {
            _elements.Add(newElement);
        }
    }
    private async Task HandleElementCommand(int id, string command)
    {
        var element = _elements.FirstOrDefault(e => e.Id == id);
        if (element is null)
        {
            return;
        }


        var isCommandValid = ValidateCommand(command);
        if (!isCommandValid)
        {
            return;
        }

        var client = _clientFactory.CreateClient();
        try
        {
            var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SmartContext>();

            var cmdResult = await client.GetAsync($"http://{element.ConnectionInfo}/relay/0?turn={command}");
            db.Log.Add(new LogItem
            {
                Type = LogItem.TYPE_COMMAND,
                ElementName = element.Name,
                ElementType = element.ElementType.ToString(),
                MetaInfo = command,
                Success = cmdResult.IsSuccessStatusCode,
                HandlerName = element.ElementHandler.Name,
            });
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
    #endregion

    #region data getter
    private async Task<ShellyStateDto?> GetState(string? connectionInfo, int id)
    {
        var client = _clientFactory.CreateClient();
        try
        {
            var result = await client.GetStringAsync($"http://{connectionInfo}/status");
            var resultObject = JsonSerializer.Deserialize<ShellyStateResult>(result);
            return new ShellyStateDto(
                id,
                resultObject!.Relays[0].Ison,
                resultObject.Time,
                DateTime.UtcNow);
        }
        catch
        {
            return null;
        }
    }
    #endregion

    #region helper
    private static bool ValidateCommand(string command)
    {
        return command switch
        {
            "on" => true,
            "off" => true,

            _ => false,
        };
    }
    #endregion

}


