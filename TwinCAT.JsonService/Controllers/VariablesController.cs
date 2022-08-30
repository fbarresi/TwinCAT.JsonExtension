using System.Reactive;
using Microsoft.AspNetCore.Mvc;

namespace TwinCAT.JsonService.Controllers;

[ApiController]
[Route("[controller]")]
public class VariablesController : ControllerBase
{
    private readonly ILogger<VariablesController> logger;

    public VariablesController(ILogger<VariablesController> logger)
    {
        this.logger = logger;
    }

    [HttpGet]
    public Task<object> Get()
    {
        return Task.FromResult<object>(new {Name = "Test", Variable = "1"});
    }
    
    [HttpPost]
    public Task Set()
    {
        return Task.FromResult(Unit.Default);
    }
}