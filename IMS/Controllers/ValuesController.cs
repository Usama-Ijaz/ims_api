using IMS.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class HelloWorldController : ControllerBase
{
    private readonly IUserContextService _userContextService;
    public HelloWorldController(IUserContextService userContextService) 
    { 
        _userContextService = userContextService;
    }
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_userContextService.GetUserId());
    }
}