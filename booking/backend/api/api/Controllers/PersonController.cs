using Microsoft.AspNetCore.Mvc;
using model;
using model.interfaces;
using model.records;
using service;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonController : ControllerBase
{
    private readonly ILogger<PersonController> _logger;
    private readonly IPersonQueryService _personQueryService;
    private readonly IPersonCommandService _personCommandService;
    
    public PersonController(
        ILogger<PersonController> logger,
        IPersonQueryService personQueryService,
        IPersonCommandService personCommandService
        )
    {
        _logger = logger;
        _personQueryService = personQueryService;
        _personCommandService = personCommandService;
    }
 
    [HttpPost]
    public async Task<IActionResult> CreatePerson([FromBody] Person person)
    {
        try
        {
            var newPerson = await _personCommandService.CreatePerson(person);
            
            return Ok(newPerson);
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
            _logger.LogError(e, "Error creating person");
            return StatusCode(500);
        }
    }
    
    [HttpGet("{personId:guid}")]
    public async Task<IActionResult> GetPerson(Guid personId)
    {
        try
        {
            var person = await _personQueryService.GetPerson(personId);
            
            return CreatedAtAction(nameof(GetPerson), new { personId }, person);
        }
        catch (RecordNotFoundException<Person> e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting person");
            return StatusCode(500);
        }
    }
}