using Microsoft.AspNetCore.Mvc;

namespace WarehouseDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Simple health-check endpoint used to verify the API host is running.
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "healthy",
            service = "WarehouseDashboard.Api",
            timestamp = DateTime.UtcNow
        });
    }
}
