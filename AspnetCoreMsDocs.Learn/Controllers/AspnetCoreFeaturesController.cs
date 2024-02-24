using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreMsDocs.Learn.Controllers;

public class AspnetCoreFeaturesController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AspnetCoreFeaturesController(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    // GET
    [HttpGet("StartupFilterReadFromQueryString")]
    public IActionResult StartupFilterReadFromQueryString([FromQuery] string option,[FromServices]HttpContextAccessor httpContextAccessor)
    {
        return Ok(option);
    }
}