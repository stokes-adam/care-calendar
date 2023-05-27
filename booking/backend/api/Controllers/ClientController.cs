using Microsoft.AspNetCore.Mvc;
using model;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientController : ControllerBase
{
    private readonly ILogger<ClientController> _logger;
    
    public ClientController(ILogger<ClientController> logger)
    {
        _logger = logger;
    }
   
    [HttpGet(Name = "GetClientsByFirm")]
    public IEnumerable<Client> GetClientsByFirm(Guid firmId)
    {
        throw new NotImplementedException();
    }
}