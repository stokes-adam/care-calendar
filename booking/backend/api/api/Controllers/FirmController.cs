using Microsoft.AspNetCore.Mvc;
using model;
using model.exceptions;
using model.interfaces;
using model.records;
using service;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class FirmController : ControllerBase
{
    private readonly ILogger<FirmController> _logger;
    private readonly IFirmService _firmService;
    
    public FirmController(ILogger<FirmController> logger, IFirmService firmService)
    {
        _logger = logger;
        _firmService = firmService;
    }
    
    [HttpGet("{firmId:guid}")]
    public async Task<IActionResult> GetFirm(Guid firmId)
    {
        try
        {
            var firm = await _firmService.GetFirm(firmId);
            
            return Ok(firm);
        }
        catch (RecordNotFoundException<Firm> e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting firm");
            return StatusCode(500, e.Message);
        }
    }
    
    
    [HttpPost]
    public async Task<IActionResult> CreateFirm([FromBody] Firm firm)
    {
        try 
        {
            var createdFirm = await _firmService.CreateFirm(firm);

            return Ok(createdFirm);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating firm");
            return StatusCode(500, e.Message);
        }
    }
}