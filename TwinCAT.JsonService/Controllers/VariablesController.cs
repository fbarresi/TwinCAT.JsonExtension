using System.Reactive;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TwinCAT.JsonExtension;
using TwinCAT.JsonService.Interfaces;

namespace TwinCAT.JsonService.Controllers;

[ApiController]
[Route("[controller]")]
public class VariablesController : ControllerBase
{
    private readonly ILogger<VariablesController> logger;
    private readonly IClientService _clientService;

    public VariablesController(ILogger<VariablesController> logger, IClientService clientService)
    {
        this.logger = logger;
        _clientService = clientService;
    }

    [HttpGet]
    [Route("{name}")]
    public async Task<object> Get(string name, [FromQuery]bool enumsToString = false)
    {
        return await _clientService.Client.ReadJson(name, force: true, stringifyEnums: enumsToString);
    }
    
    [HttpPost]
    [Route("{name}")]
    public async Task Set(string name, object value)
    {
        var json = new JObject(value);
        await _clientService.Client.WriteJson(name, json, force: true);
    }
    
    [HttpPost]
    [Route("list/{name}")]
    public async Task Set(string name, IEnumerable<object> value)
    {
        var json = new JArray(value);
        await _clientService.Client.WriteJson(name, json, force: true);
    }
}