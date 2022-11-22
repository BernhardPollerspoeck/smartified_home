using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using smart.contract;
using smart.database;
using smart.resources;
using System.Diagnostics;

namespace smart.api.Hubs;

public class HandlerHub : Hub
{
    #region fields
    private readonly SmartContext _context;
    #endregion

    #region ctor
    public HandlerHub(SmartContext context)
    {
        _context = context;
    }
    #endregion

    #region overrides
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.Items.TryGetValue("handlerId", out var value))
        {//set disconnected
            var handlerId = (int)value!;
            var handler = await _context
                .ElementHandlers
                .FirstOrDefaultAsync(h => h.Id == handlerId);
            if (handler is not null)
            {
                handler.Connected = false;
                _context.Log.Add(new LogItem
                {
                    Type = $"{SmartResources.Handler}: {handler.HandlerType}",
                    HandlerName = handler.Name,
                    MetaInfo = SmartResources.Log_handler_offline,
                    Timestamp = DateTime.UtcNow,
                });
                await _context.SaveChangesAsync();
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        _context.Log.Add(new LogItem
        {
            HandlerName = Context.ConnectionId,
            MetaInfo = SmartResources.Log_anonymous_connection,
            Timestamp = DateTime.UtcNow,
        });
        await _context.SaveChangesAsync();
    }
    #endregion

    #region static helper
    internal static async Task NewElement(IHubContext<HandlerHub> hubContext, int id, string connectionInfo, int handlerId)
    {
        await hubContext
             .Clients
             .Group($"handler {handlerId}")
             .SendAsync(nameof(NewElement), id, connectionInfo);
    }
    internal static async Task SendElementCommand(IHubContext<HandlerHub> hubContext, int id, string command, int handlerId)
    {
        await hubContext
            .Clients
            .Group($"handler {handlerId}")
            .SendAsync(nameof(SendElementCommand), id, command);
    }
    internal static async Task PollElements(IHubContext<HandlerHub> hubContext, int handlerId)
    {
        await hubContext
            .Clients
            .Group($"handler {handlerId}")
            .SendAsync(nameof(PollElements));
    }
    #endregion

    #region incomming requests
    public async Task ReportHandlerType(int handlerId, EHandlerType handlerType)
    {
        Debug.WriteLine($"Handler Reporting {handlerId} {handlerType}");
        var handler = await _context
            .ElementHandlers
            .FirstOrDefaultAsync(h => h.Id == handlerId
                && h.HandlerType == handlerType);
        if (handler is null)
        {
            return;
        }
        Debug.WriteLine($"Handler {handlerId} {handlerType}");

        handler.Connected = true;

        _context.Log.Add(new LogItem
        {
            Type = $"{SmartResources.Handler}: {handler.HandlerType}",
            HandlerName = $"{handler.Name} - {Context.ConnectionId}",
            MetaInfo = SmartResources.Log_handler_online,
            Timestamp = DateTime.UtcNow,
        });
        await _context.SaveChangesAsync();
        Context.Items.Add("handlerId", handler.Id);
        await Groups.AddToGroupAsync(Context.ConnectionId, $"handler {handlerId}");
    }
    public Task ElementStatesChanged(List<int> elements)
    {
        //TODO: notify app clients

        return Task.CompletedTask;
    }
    #endregion

}
