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
    
    [HttpGet("{firmId:guid}")]
    public async Task<IActionResult> GetFirm(Guid firmId)
    {
        try
        {
            var firm = await _firmQueryService.GetFirm(firmId);
            
            return CreatedAtAction(nameof(GetFirm), new { firmId }, firm);
        }
        catch (RecordNotFoundException<Firm> e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting firm");
            return StatusCode(500);
        }
    }
    
    
    [HttpPost]
    public async Task<IActionResult> CreateFirm([FromBody] Firm firm)
    {
        try
        {
            var createdFirm = await _firmCommandService.CreateFirm(firm);
            
            return CreatedAtAction(nameof(CreateFirm), new { firmId = createdFirm.Id }, createdFirm);
        }
        catch (AccessViolationException e)
        {
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating firm");
            return StatusCode(500);
        }
    }
    
    [HttpPost("{firmId:guid}/person/{personId:guid}/{role}")]
    public async Task<IActionResult> AssignFirmRoleToPerson(Guid firmId, Guid personId, string role)
    {
        try
        {
            await _firmCommandService.AssignFirmRoleToPerson(firmId, personId, role);
            
            return Ok();
        }
        catch (AccessViolationException e)
        {
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error adding person to firm");
            return StatusCode(500);
        }
    }
    
    [HttpDelete("{firmId:guid}/person/{personId:guid}/{role}")]
    public async Task<IActionResult> RemoveFirmRoleFromPerson(Guid firmId, Guid personId, string role)
    {
        try
        {
            await _firmCommandService.RemoveFirmRoleFromPerson(firmId, personId, role);
            
            return Ok();
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
            _logger.LogError(e, "Error removing person from firm");
            return StatusCode(500);
        }
    }
}