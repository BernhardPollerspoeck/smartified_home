using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using smart.contract;
using smart.contract.Handler;
using smart.core.Models;
using smart.handler.shelly.Models;
using smart.resources;
using System.Net.WebSockets;
using System.Text.Json;
using System.Xml.Linq;

namespace smart.handler.shelly.Services;

internal class MainHandler : BackgroundService
{
    #region fields
    private readonly List<StateElement> _elements;

    private readonly IHttpClientFactory _clientFactory;
    private readonly int _id;
    private HubConnection? _hubConnection;
    #endregion

    #region ctor
    public MainHandler(
        IConfiguration configuration,
        IHttpClientFactory clientFactory)
    {
        _elements = new();
        _clientFactory = clientFactory;
        _id = configuration.GetValue<int>("id");
    }
    #endregion

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
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
                    Console.WriteLine($"{DateTime.Now} Trying connection");
                    await _hubConnection.StartAsync(cts.Token);
                }
                catch { }//continue

            }
            await _hubConnection.SendAsync("ReportHandlerType", _id, EHandlerType.Shelly, cancellationToken: stoppingToken);

        };

        _hubConnection.On<StateElement, string>("SendElementCommand", HandleElementCommand);
        _hubConnection.On("PollElements", HandlePollElements);
        _hubConnection.On<IEnumerable<StateElement>>("ElementInfo", HandleNewElement);

        await _hubConnection.StartAsync(stoppingToken);

        await _hubConnection.SendAsync("ReportHandlerType", _id, EHandlerType.Shelly, cancellationToken: stoppingToken);
    }

    #region signalR handler
    private Task HandleNewElement(IEnumerable<StateElement> element)
    {
        _elements.AddRange(element);
        return PollState(element);
    }

    private async Task HandleElementCommand(StateElement element, string command)
    {
        var isCommandValid = ValidateCommand(command);
        if (!isCommandValid)
        {
            return;
        }

        var client = _clientFactory.CreateClient();
        try
        {
            var cmdResult = await client.GetAsync($"http://{element.Connection}/relay/0?turn={command}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
    private Task HandlePollElements()
    {//todo: only poll required elements
        return PollState(_elements);
    }
    #endregion

    #region state handling
    private async Task PollState(IEnumerable<StateElement> elementsToPoll)
    {
        var changed = new List<StateElement>();

        foreach (var item in elementsToPoll)
        {
            var shellyState = await GetState(item.Connection, item.Id);
            if (shellyState is not null)
            {
                if (shellyState.IsOn == (item.Properties[StateElement.IS_ON] as bool?))
                {//no state change

                }
                else
                {
                    item.Properties[StateElement.IS_ON] = shellyState.IsOn;
                    item.State = JsonSerializer.Serialize(shellyState);
                    changed.Add(item);
                }
            }
        }
        if (_hubConnection is not null)
        {
            await _hubConnection.SendAsync("ElementStatesChanged", changed);
        }

    }
    private async Task<ShellyStateDto?> GetState(string? connectionInfo, int id)
    {
        var client = _clientFactory.CreateClient();
        try
        {
            var result = await client.GetStringAsync($"http://{connectionInfo}/status");
            var resultObject = JsonSerializer.Deserialize<ShellyStateResult>(result);
            return new ShellyStateDto
            {
                Id = id,
                IsOn = resultObject!.Relays[0].Ison,
                DeviceTime = resultObject.Time,
                StateTimestamp = DateTime.UtcNow
            };
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


