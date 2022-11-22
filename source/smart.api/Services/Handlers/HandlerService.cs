using smart.contract;
using smart.core.Models;
using smart.database;
using smart.resources;
using System.Threading.Channels;

namespace smart.api.Services.Handlers;

public class HandlerService
{
    private readonly SmartContext _context;
    private readonly ChannelWriter<ElementHandler> _channelWriter;

    public HandlerService(
        SmartContext context,
        ChannelWriter<ElementHandler> channelWriter)
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
                h.HandlerType,
                h.Connected)));
    }

    public async Task<HandlerDto> Create(CreateHandlerDto dto)
    {
        if (_context.ElementHandlers.Any(h => h.HandlerType == dto.HandlerType))
        {
            throw new AppException(SmartResources.Api_Ex_handler_already_existing);
        }
        var newHandler = new ElementHandler
        {
            Name = dto.Name,
            HandlerType = dto.HandlerType,
            Connected = false,
        };
        _context.ElementHandlers.Add(newHandler);
        _context.Log.Add(new LogItem
        {
            HandlerName = dto.Name,
            Type = dto.HandlerType.ToString(),
            MetaInfo = SmartResources.Log_create_handler,
            Timestamp = DateTime.UtcNow,
        });
        await _context.SaveChangesAsync();
        await _channelWriter.WriteAsync(newHandler);

        return new HandlerDto(
            newHandler.Id,
            newHandler.Name,
            newHandler.HandlerType,
            newHandler.Connected);
    }
}
