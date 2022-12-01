using bp.net.Auth.Server.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace smart.api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PlanController : BaseController
{




    //create plan

    //edit plan

    //get plans

    //get plan

}

[Authorize]
[ApiController]
[Route("[controller]")]
public class PlanElementController : BaseController
{


    //create element on plan

    //move element

    //edit element

    //get elements for plan

}
