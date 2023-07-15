using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TwinCAT.JsonExtension;
using TwinCAT.JsonService.Interfaces;
using TwinCAT.TypeSystem;

namespace TwinCAT.JsonService.Controllers;

public class SymbolsController : ControllerBase
{
    private readonly ILogger<SymbolsController> logger;
    private readonly IClientService clientService;

    public SymbolsController(ILogger<SymbolsController> logger, IClientService clientService)
    {
        this.logger = logger;
        this.clientService = clientService;
    }

    [HttpGet]
    [Route("tree")]
    public Task<ISymbolCollection<ISymbol>> GetTree()
    {
        return Task.FromResult(clientService.TreeViewSymbols);
    }
    
    [HttpPost]
    [Route("flat")]
    public Task<ISymbolCollection<ISymbol>> GetFlat()
    {
        return Task.FromResult(clientService.FlatViewSymbols);
    }
    
    [HttpPost]
    [Route("get/{path}")]
    public Task<IEnumerable<ISymbol>> StartingWith(string path)
    {
        return Task.FromResult(clientService.FlatViewSymbols.Where(s => s.InstancePath.StartsWith(path)));
    }
    
    [HttpPost]
    [Route("query/{path}")]
    public Task<IEnumerable<ISymbol>> query(string name)
    {
        return Task.FromResult(clientService.FlatViewSymbols.Where(s => s.InstancePath.Contains(name)));
    }
}