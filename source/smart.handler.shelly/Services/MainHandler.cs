using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using smart.contract;
using smart.contract.Handler;
using smart.core.Models;
using smart.handler.shelly.Models;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text.Json;
using System.Xml.Linq;

namespace smart.handler.shelly.Services;

internal class MainHandler : BackgroundService, IHandlerClient
{
    #region fields
    private readonly List<StateElement> _elements;

    private readonly IHttpClientFactory _clientFactory;
    private readonly int _id;
    private readonly string _hubUrl;
    private CancellationToken _serviceCancellationToken;
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
        _hubUrl = configuration.GetValue<string>("hub");
    }
    #endregion

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _serviceCancellationToken = stoppingToken;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{_hubUrl}/handlerHub")
            .Build();
        _hubConnection.Closed += HandleConnectionClosed;

        _hubConnection.On<IEnumerable<StateElement>>(nameof(IHandlerClient.OnNewElements), OnNewElements);
        _hubConnection.On<IEnumerable<StateElement>>(nameof(IHandlerClient.OnPollRequest), OnPollRequest);
        _hubConnection.On<StateElement, string>(nameof(IHandlerClient.OnElementCommand), OnElementCommand);

        await _hubConnection.StartAsync(stoppingToken);

        await _hubConnection.SendAsync(nameof(IHandlerHub.OnHandlerAlive), _id, EHandlerType.Shelly, cancellationToken: stoppingToken);
    }

    #region signalR handler
    private async Task HandleConnectionClosed(Exception? ex)
    {
        if (_hubConnection is null)
        {
            Console.WriteLine("Connection is null. cant reconnect");
            return;
        }
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
        await _hubConnection.SendAsync(nameof(IHandlerHub.OnHandlerAlive), _id, EHandlerType.Shelly, _serviceCancellationToken);
    }

    public Task OnNewElements(IEnumerable<StateElement> elements)
    {
        _elements.AddRange(elements);
        return PollState(elements);
    }
    public Task OnPollRequest(IEnumerable<StateElement> elements)
    {
        return PollState(elements);
    }
    public async Task OnElementCommand(StateElement element, string command)
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
    #endregion

    #region state handling
    private async Task PollState(IEnumerable<StateElement> elementsToPoll)
    {
        var changed = new List<StateElement>();

        foreach (var item in elementsToPoll)
        {
            if (!_elements.Any(e => e.Id == item.Id))
            {//dont poll unknown elements
                continue;
            }
            var shellyState = await GetState(item.Connection, item.Id);
            if (shellyState is not null)
            {
                if (shellyState.IsOn != (item.Properties[StateElement.IS_ON] as bool?))
                {
                    item.Properties[StateElement.IS_ON] = shellyState.IsOn;
                    item.State = JsonSerializer.Serialize(shellyState);
                    changed.Add(item);
                }
            }
        }
        if (_hubConnection is not null)
        {
            await _hubConnection.SendAsync(nameof(IHandlerHub.OnElementStatesChanged), changed);
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


