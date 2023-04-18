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
    private readonly IClientService clientService;

    public VariablesController(ILogger<VariablesController> logger, IClientService clientService)
    {
        this.logger = logger;
        this.clientService = clientService;
    }

    [HttpGet]
    [Route("{name}")]
    public async Task<object> Get(string name, [FromQuery]bool enumsToString = false)
    {
        logger.LogInformation("Trying to get {variable} (enumsToString={enumsToString})...", name, enumsToString);
        var start = DateTime.Now;
        var json = await clientService.Client.ReadJson(name, force: true, stringifyEnums: enumsToString);
        logger.LogInformation("Var {variable} read in {readTime}", name, DateTime.Now-start);
        return json;
    }
    
    [HttpPost]
    [Route("{name}")]
    public async Task Set(string name, object value)
    {
        logger.LogInformation("Trying to set {variable}...", name);
        var start = DateTime.Now;
        var json = new JObject(value);
        await clientService.Client.WriteJson(name, json, force: true);
        logger.LogInformation("Var {variable} set in {readTime}", name, DateTime.Now-start);
    }
    
    [HttpPost]
    [Route("list/{name}")]
    public async Task Set(string name, IEnumerable<object> value)
    {
        logger.LogInformation("Trying to set list {variable}...", name);
        var start = DateTime.Now;
        var json = new JArray(value);
        await clientService.Client.WriteJson(name, json, force: true);
        logger.LogInformation("list {variable} set in {readTime}", name, DateTime.Now-start);
    }
}