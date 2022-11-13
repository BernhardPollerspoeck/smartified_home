using Microsoft.EntityFrameworkCore;
using smart.core.Models;
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
                h.Connected)));
    }

    public async Task<HandlerDto> Create(CreateHandlerDto dto)
    {
        var newHandler = new ElementHandler
        {
            Name = dto.Name,
            ElementType = (EElementType)dto.HandlerType,
            Connected = false,
        };
        _context.ElementHandlers.Add(newHandler);
        _context.Log.Add(new LogItem
        {
            HandlerName = dto.Name,
            ElementType = dto.HandlerType.ToString(),
            MetaInfo = SmartResources.Log_create_handler,
            Timestamp = DateTime.UtcNow,
        });
        await _context.SaveChangesAsync();

        return new HandlerDto(
            newHandler.Id,
            newHandler.Name,
            (EHandlerType)newHandler.ElementType,
            newHandler.Connected);
    }
}
