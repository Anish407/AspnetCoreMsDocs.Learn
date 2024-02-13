using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreMsDocs.Learn.Controllers;

public class AspnetCoreFeaturesController : ControllerBase
{
    // GET
    [HttpGet("StartupFilterReadFromQueryString")]
    public IActionResult StartupFilterReadFromQueryString([FromQuery] string option)
    {
        return Ok(option);
    }
}