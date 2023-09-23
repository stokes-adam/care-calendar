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

    public PersonController(ILogger<PersonController> logger)
    {
        _logger = logger;
    }
}