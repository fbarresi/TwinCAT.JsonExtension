using Microsoft.Extensions.Logging;
using Moq;
using TwinCAT.JsonService.Controllers;
using TwinCAT.JsonService.Interfaces;

namespace TwinCAT.JsonService.Tests;

public class TestSymbolsController
{
    [SetUp]
    public void Setup()
    {
        var logger = new Mock<ILogger<SymbolsController>>();
        var clientService = new Mock<ILogger<SymbolsController>>();
    }

    [Test]
    public async Task TestGetTree()
    {
        var logger = new Mock<ILogger<SymbolsController>>();
        var clientService = new Mock<IClientService>();

        var controller = new SymbolsController(logger.Object, clientService.Object);
        await controller.GetTree();
        
        clientService.Verify(s => s.TreeViewSymbols, Times.Once);
    }
}