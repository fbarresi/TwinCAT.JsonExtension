using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TwinCAT.JsonService.Interfaces;

namespace TwinCAT.JsonService.Controllers;

public class StatusController : ControllerBase
{
    private readonly ILogger<StatusController> logger;
    private readonly IClientService clientService;

    public StatusController(ILogger<StatusController> logger, IClientService clientService)
    {
        this.logger = logger;
        this.clientService = clientService;
    }
    [HttpGet]
    [Route("connection")]
    public Task<ConnectionState> GetConnectionState()
    {
        return clientService.ConnectionState.FirstAsync().ToTask();
    }
    [HttpGet]
    [Route("ads")]
    public Task<string> GetAdsState()
    {
        return clientService.AdsState.FirstAsync().ToTask();
    }
}