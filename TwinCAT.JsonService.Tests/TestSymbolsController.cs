using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.JsonExtension.Tests;
using TwinCAT.JsonService.Controllers;
using TwinCAT.JsonService.Interfaces;
using TwinCAT.TypeSystem;

namespace TwinCAT.JsonService.Tests;

public class TestSymbolsController
{
    private Mock<ILogger<SymbolsController>> logger;
    private Mock<IClientService> clientService;

    [SetUp]
    public void Setup()
    {
        logger = new Mock<ILogger<SymbolsController>>();
        clientService = new Mock<IClientService>();
    }

    [Test]
    public async Task TestGetTree()
    {
        var controller = new SymbolsController(logger.Object, clientService.Object);
        await controller.GetTree();
        
        clientService.Verify(s => s.TreeViewSymbols, Times.Once);
    }
    
    [Test]
    public async Task TestGetFlat()
    {
        var controller = new SymbolsController(logger.Object, clientService.Object);
        await controller.GetFlat();
        
        clientService.Verify(s => s.FlatViewSymbols, Times.Once);
    }
    
    [Test]
    public async Task TestGet()
    {
        clientService.Setup(s => s.FlatViewSymbols)
            .Returns(new SymbolCollection() { new DebugSymbol() {Name = "test01"} })
            ;
        
        var controller = new SymbolsController(logger.Object, clientService.Object);
        var symbols = await controller.StartingWith("test");

        clientService.Verify(s => s.FlatViewSymbols, Times.Once);
        symbols.Count().ShouldBe(1);
    }
    
}