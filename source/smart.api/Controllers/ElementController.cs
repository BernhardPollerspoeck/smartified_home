using bp.net.Auth.Server.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using smart.api.Hubs;
using smart.contract;
using smart.core.Models;
using smart.database;
using smart.resources;
using System.Net.Mime;

namespace smart.api.Controllers;


[Authorize]
[ApiController]
[Route("[controller]")]
public class ElementController : BaseController
{
    #region fields
    private readonly IHubContext<HandlerHub> _hubContext;
    private readonly SmartContext _context;
    #endregion

    #region ctor
    public ElementController(
        IHubContext<HandlerHub> hubContext,
        SmartContext context)
    {
        _hubContext = hubContext;
        _context = context;
    }
    #endregion

    [HttpPost]
    public async Task<IActionResult> Post(CreateElementDto dto)
    {
        var handler = await _context
            .ElementHandlers
            .FirstOrDefaultAsync(h => h.HandlerType == dto.HandlerType);
        if (handler is null)
        {
            throw new AppException(SmartResources.Api_Ex_handler_not_found);
        }

        var element = new HomeElement
        {
            ElementHandler = handler,
            ElementType = dto.ElementType,
            Name = dto.Name,
            ConnectionInfo = dto.ConnectionInfo,
        };
        _context.Elements.Add(element);
        _context.Log.Add(new LogItem
        {
            ElementName = dto.Name,
            Type = $"{SmartResources.Element}: {dto.ElementType}",
            HandlerName = handler.Name,
            MetaInfo = SmartResources.Log_create_element,
            Timestamp = DateTime.UtcNow,
        });
        await _context.SaveChangesAsync();

        await HandlerHub.ElementInfo(_hubContext, new[]
        {
            new StateElement
            {
                Id = element.Id,
                State = element.StateData,
                Connection= dto.ConnectionInfo
            }
        }, handler.Id);

        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Put(ElementCommandDto dto)
    {
        var handler = await _context
            .ElementHandlers
            .Include(h => h.HomeElements)
            .FirstOrDefaultAsync(h => h.HomeElements.Any(e => e.Id == dto.Id));
        if (handler is null)
        {
            throw new AppException(SmartResources.Api_Ex_handler_not_found);
        }
        var element = handler.HomeElements.FirstOrDefault(e => e.Id == dto.Id);
        if (element is null)
        {
            throw new AppException(SmartResources.Api_Ex_element_not_found);
        }


        await HandlerHub
            .SendElementCommand(
                _hubContext,
                new StateElement
                {
                    Id = element.Id,
                    State = element.StateData,
                    Connection = element.ConnectionInfo,
                },
                dto.Command,
                handler.Id);
        return Ok();
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_context
            .Elements
            .Select(e => new ElementDto(
                e.Id,
                e.Name,
                e.ElementType,
                e.ConnectionValidated)));
    }

    [HttpGet("{elementId}")]
    public async Task<IActionResult> Get(int elementId)
    {
        var element = await _context
            .Elements
            .FirstOrDefaultAsync(e => e.Id == elementId);
        return element is null
            ? throw new AppException(SmartResources.Api_Ex_element_not_found)
            : (IActionResult)Content(element.StateData ?? string.Empty, MediaTypeNames.Application.Json);
    }

}
