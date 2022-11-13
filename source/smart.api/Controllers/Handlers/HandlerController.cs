using bp.net.Auth.Server.Attributes;
using Microsoft.AspNetCore.Mvc;
using smart.api.Services.Handlers;
using smart.contract;
using System.Security.AccessControl;

namespace smart.api.Controllers.Handlers;




[Authorize]
[ApiController]
[Route("[controller]")]
public class HandlerController : BaseController
{
    private readonly HandlerService _handlerService;

    public HandlerController(HandlerService handlerService)
    {
        _handlerService = handlerService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _handlerService.Get();
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateHandlerDto dto)
    {
        var result = await _handlerService.Create(dto);
        return Ok(result);
    }

   
}
