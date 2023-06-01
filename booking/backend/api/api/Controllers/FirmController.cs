using Microsoft.AspNetCore.Mvc;
using model;
using model.interfaces;
using model.records;
using service;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class FirmController : ControllerBase
{
    private readonly ILogger<FirmController> _logger;
    private readonly IFirmQueryService _firmQueryService;
    
    public FirmController(ILogger<FirmController> logger, IFirmQueryService firmQueryService)
    {
        _logger = logger;
        _firmQueryService = firmQueryService;
    }
    
    [HttpGet("owner/{email}")]
    public async Task<IActionResult> GetFirmsForOwnerPersonWithEmail(string email)
    {
        return Ok();
    }
}