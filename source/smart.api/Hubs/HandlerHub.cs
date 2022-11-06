using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using smart.contract;
using smart.database;

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
        if (Context.Items.ContainsKey("handlerId"))
        {//set disconnected
            var handlerId = (int)Context.Items["handlerId"]!;
            var handler = await _context
                .ElementHandlers
                .FirstOrDefaultAsync(h => h.Id == handlerId);
            if (handler is not null)
            {
                handler.Connected = false;
                await _context.SaveChangesAsync();
            }
        }

        await base.OnDisconnectedAsync(exception);
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
    #endregion

    #region incomming requests
    public async Task ReportHandlerType(int handlerId, EHandlerType handlerType)
    {
        var handler = await _context
            .ElementHandlers
            .FirstOrDefaultAsync(h => h.Id == handlerId
                && ((EHandlerType)h.ElementType) == handlerType);
        if (handler is null)
        {
            return;
        }

        handler.Connected = true;
        await _context.SaveChangesAsync();
        Context.Items.Add("handlerId", handler.Id);
        await Groups.AddToGroupAsync(Context.ConnectionId, $"handler {handlerId}");
    }
    #endregion

}
