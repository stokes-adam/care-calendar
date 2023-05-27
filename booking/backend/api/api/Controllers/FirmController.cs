using Microsoft.AspNetCore.Mvc;
using model;
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
        try
        {
            var firms = await _firmQueryService.GetForOwnerPersonWithEmail(email);
            
            return Ok(firms);
        }
        catch (AccessViolationException e)
        {
            return Unauthorized(e.Message);
        }
        catch (RecordNotFoundException<Firm> e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting firms for owner person with email {Email}", email);
            return StatusCode(500);
        }
    }
}