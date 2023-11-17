using Microsoft.AspNetCore.Mvc;
using model;
using model.dtos;
using model.interfaces;
using model.records;
using service;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonController : ControllerBase
{
    private readonly ILogger<PersonController> _logger;
    private readonly IPersonService _personService;

    public PersonController(ILogger<PersonController> logger, IPersonService personService)
    {
        _logger = logger;
        _personService = personService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreatePerson([FromBody] CreatePersonDetail createPersonDto)
    {
        try
        {
            var personId = await _personService.CreatePerson();

            var personDetailId = await _personService.CreatePersonDetail(personId, createPersonDto);
            
            return Ok(new { personId, personDetailId });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create person");
            
            return StatusCode(500, e.Message);
        }
    }
}