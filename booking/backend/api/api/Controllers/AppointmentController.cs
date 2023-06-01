using Microsoft.AspNetCore.Mvc;
using model;
using model.records;
using service;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly ILogger<AppointmentController> _logger;
    
    public AppointmentController(ILogger<AppointmentController> logger)
    {
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] Appointment appointment)
    {
        try
        {
            return Ok(appointment);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating appointment");
            return StatusCode(500);
        }
    }
}