using bp.net.Auth.Server.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using smart.api.Hubs;

namespace smart.api.Controllers;



[Authorize]
[ApiController]
[Route("[controller]")]
public class TestController : BaseController
{

    private readonly IHubContext<HandlerHub> _hubContext;

    public TestController(IHubContext<HandlerHub> hubContext)
    {
        _hubContext = hubContext;
    }


    [HttpPost("{handlerId}")]
    public async Task<IActionResult> Poll(int handlerId)
    {
        await HandlerHub.PollElements(_hubContext, handlerId);
        return Ok();
    }

}
