using bp.net.Auth.Server.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using smart.api.Hubs;
using smart.core.Models;
using smart.database;

namespace smart.api.Controllers;



[Authorize]
[ApiController]
[Route("[controller]")]
public class TestController : BaseController
{

    private readonly IHubContext<HandlerHub> _hubContext;
    private readonly SmartContext _context;

    public TestController(
        IHubContext<HandlerHub> hubContext,
        SmartContext context)
    {
        _hubContext = hubContext;
        _context = context;
    }


    [HttpPost("{handlerId}")]
    public async Task<IActionResult> Poll(int handlerId)
    {
        var elements = _context
            .ElementHandlers
            .Include(h => h.HomeElements)
            .First(h => h.Id == handlerId)
            .HomeElements.Select(e => new StateElement
            {
                Id = e.Id,
                Connection = e.ConnectionInfo,
                State = e.StateData,
            });
        await HandlerHub.PollElements(_hubContext, elements, handlerId);
        return Ok();
    }

}
