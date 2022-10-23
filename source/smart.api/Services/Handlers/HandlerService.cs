using Microsoft.EntityFrameworkCore;
using smart.api.Models;
using smart.api.Services.Handlers.ProcessControlling;
using smart.contract;
using smart.database;
using smart.resources;
using System.Threading.Channels;

namespace smart.api.Services.Handlers;

public class HandlerService
{
    private readonly SmartContext _context;
    private readonly ChannelWriter<HandlerControlMessage> _channelWriter;

    public HandlerService(
        SmartContext context,
        ChannelWriter<HandlerControlMessage> channelWriter)
    {
        _context = context;
        _channelWriter = channelWriter;
    }

    public Task<IEnumerable<HandlerDto>> Get()
    {
        return Task.FromResult<IEnumerable<HandlerDto>>(_context
            .ElementHandlers
            .Select(h => new HandlerDto(
                h.Id,
                h.Name,
                (EHandlerType)h.ElementType,
                h.Enabled,
                h.ProcessRunning,
                h.SignalConnected)));
    }

    public async Task<HandlerDto> Create(CreateHandlerDto dto)
    {
        var newHandler = new ElementHandler
        {
            Name = dto.Name,
            ElementType = (EElementType)dto.HandlerType,
            ProcessRunning = false,
            SignalConnected = false,
        };
        _context.ElementHandlers.Add(newHandler);
        await _context.SaveChangesAsync();
        return new HandlerDto(
            newHandler.Id,
            newHandler.Name,
            (EHandlerType)newHandler.ElementType,
            newHandler.Enabled,
            newHandler.ProcessRunning,
            newHandler.SignalConnected);
    }

    public async Task<HandlerDto> Enable(int handlerId, bool enabled)
    {
        #region update database value
        var existing = await _context
            .ElementHandlers
            .FirstOrDefaultAsync(h => h.Id == handlerId);
        if (existing is null)
        {
            throw new AppException(SmartResources.Api_Ex_handler_not_found);
        }
        existing.Enabled = enabled;
        await _context.SaveChangesAsync();
        #endregion

        #region notify handler control service
        await _channelWriter.WriteAsync(
            new HandlerControlMessage(
                enabled ? EHandlerAction.Enabled : EHandlerAction.Disabled,
                handlerId));
        #endregion

        return new HandlerDto(
            existing.Id,
            existing.Name,
            (EHandlerType)existing.ElementType,
            existing.Enabled,
            existing.ProcessRunning,
            existing.SignalConnected);
    }

}
