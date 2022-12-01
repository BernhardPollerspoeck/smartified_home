using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using smart.contract;
using smart.core.Models;
using smart.database;
using smart.resources;
using System.Diagnostics;
using System.Linq.Expressions;

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
        if (!Context.Items.TryGetValue("handlerId", out var value))
        {
            await base.OnDisconnectedAsync(exception);
            return;
        }

        #region set disconnected
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
        #endregion

        await base.OnDisconnectedAsync(exception);
    }
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        #region log incomming connection
        _context.Log.Add(new LogItem
        {
            HandlerName = Context.ConnectionId,
            MetaInfo = SmartResources.Log_anonymous_connection,
            Timestamp = DateTime.UtcNow,
        });
        await _context.SaveChangesAsync();
        #endregion
    }
    #endregion

    #region static helper
    internal static async Task ElementInfo(IHubContext<HandlerHub> hubContext, IEnumerable<StateElement> elements, int handlerId)
    {
        await hubContext
             .Clients
             .Group($"handler {handlerId}")
             .SendAsync(nameof(IHandlerClient.OnNewElements), elements);
    }
    internal static async Task SendElementCommand(IHubContext<HandlerHub> hubContext, StateElement element, string command, int handlerId)
    {
        await hubContext
            .Clients
            .Group($"handler {handlerId}")
            .SendAsync(nameof(IHandlerClient.OnElementCommand), element, command);
    }
    internal static async Task PollElements(IHubContext<HandlerHub> hubContext, IEnumerable<StateElement> elements, int handlerId)
    {
        await hubContext
            .Clients
            .Group($"handler {handlerId}")
            .SendAsync(nameof(IHandlerClient.OnPollRequest), elements);
    }
    #endregion

    #region incomming requests
    public async Task OnHandlerAlive(int handlerId, EHandlerType handlerType)
    {
        Debug.WriteLine($"Handler Reporting {handlerId} {handlerType}");
        
        #region get handler instance
        var handler = await _context
            .ElementHandlers
            .Include(h => h.HomeElements)
            .FirstOrDefaultAsync(h => h.Id == handlerId
                && h.HandlerType == handlerType);
        if (handler is null)
        {
            Debug.WriteLine("Invalid handler");
            return;
        }
        #endregion

        Debug.WriteLine($"Handler valid {handlerId} {handlerType}");
        handler.Connected = true;

        #region log the handler alive
        _context.Log.Add(new LogItem
        {
            Type = $"{SmartResources.Handler}: {handler.HandlerType}",
            HandlerName = $"{handler.Name} - {Context.ConnectionId}",
            MetaInfo = SmartResources.Log_handler_online,
            Timestamp = DateTime.UtcNow,
        });
        await _context.SaveChangesAsync();
        #endregion

        #region supply data to context
        Context.Items.Add("handlerId", handler.Id);
        var group = $"handler {handlerId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
        #endregion

        #region send handler his initial elements
        await Clients
            .Group(group)
            .SendAsync(
                "ElementInfo",
                handler
                    .HomeElements
                    .Select(e => new StateElement
                    {
                        Id = e.Id,
                        State = e.StateData,
                        Connection = e.ConnectionInfo,
                    }));
        #endregion
    }
    public async Task OnElementStatesChanged(List<StateElement> elements)
    {
        //TODO: notify app clients
        foreach (var item in elements)
        {
            var dbElement = await _context.Elements.FirstOrDefaultAsync(e => e.Id == item.Id);
            if (dbElement is null)
            {
                continue;
            }

            dbElement.StateData = item.State;
            dbElement.StateTimestamp = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
    }
    #endregion

}
