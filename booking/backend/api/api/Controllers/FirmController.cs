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
    private readonly IFirmCommandService _firmCommandService;
    
    public FirmController(
        ILogger<FirmController> logger,
        IFirmQueryService firmQueryService,
        IFirmCommandService firmCommandService)
        
    {
        _logger = logger;
        _firmQueryService = firmQueryService;
        _firmCommandService = firmCommandService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateFirm([FromBody] Firm firm)
    {
        try
        {
            var createdFirm = await _firmCommandService.CreateFirm(firm);
            
            return CreatedAtAction(nameof(CreateFirm), new { firmId = createdFirm.Id }, createdFirm);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating firm");
            return StatusCode(500);
        }
    }
    
    
    [HttpGet("owner/{email}")]
    public async Task<IActionResult> GetFirmsForOwnerPersonWithEmail(string email)
    {
        return Ok();
    }
}