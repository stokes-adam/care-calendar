using Microsoft.AspNetCore.Mvc;
using model;
using service;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientController : ControllerBase
{
    private readonly ILogger<ClientController> _logger;
    private readonly IClientQueryService _clientQueryService;
    
    public ClientController(ILogger<ClientController> logger, IClientQueryService clientQueryService)
    {
        _logger = logger;
        _clientQueryService = clientQueryService;
    }
   
    [HttpGet("firm/{firmId:guid}")]
    public async Task<IActionResult> GetClientsForFirm(Guid firmId)
    {
        try
        {
            var clients = await _clientQueryService.GetClientsForFirm(firmId);
            
            return Ok(clients);
        }
        catch (AccessViolationException e)
        {
            return Unauthorized(e.Message);
        }
        catch (RecordNotFoundException<Client> e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting clients for firm {FirmId}", firmId);
            return StatusCode(500);
        }
    }
    
    [HttpGet("{id:guid}", Name = "GetClient")]
    public async Task<IActionResult> GetClient(Guid id)
    {
        try
        {
            var client = await _clientQueryService.GetClient(id);
            
            return Ok(client);
        }
        catch (AccessViolationException e)
        {
            return Unauthorized(e.Message);
        }
        catch (RecordNotFoundException<Client> e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting client with id {Id}", id);
            return StatusCode(500);
        }
    }
}