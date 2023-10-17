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
    private SymbolsController controller;

    [SetUp]
    public void Setup()
    {
        logger = new Mock<ILogger<SymbolsController>>();
        clientService = new Mock<IClientService>();
        controller = new SymbolsController(logger.Object, clientService.Object);

    }

    [Test]
    public async Task TestGetTree()
    {
        await controller.GetTree();
        
        clientService.Verify(s => s.TreeViewSymbols, Times.Once);
    }
    
    [Test]
    public async Task TestGetFlat()
    {
        await controller.GetFlat();
        
        clientService.Verify(s => s.FlatViewSymbols, Times.Once);
    }
    
    [Test]
    public async Task TestGet()
    {
        clientService.Setup(s => s.FlatViewSymbols)
            .Returns(new SymbolCollection() { new DebugSymbol() {Name = "test01"} })
            ;
        
        var symbols = await controller.GetByInstancePath("test");

        clientService.Verify(s => s.FlatViewSymbols, Times.Once);
        symbols.Count().ShouldBe(1);
    }
    
    [Test]
    public async Task TestQuery()
    {
        clientService.Setup(s => s.FlatViewSymbols)
            .Returns(new SymbolCollection() { new DebugSymbol() {Name = "test01"} })
            ;
        
        var symbols = await controller.QueryInstancePath("01");

        clientService.Verify(s => s.FlatViewSymbols, Times.Once);
        symbols.Count().ShouldBe(1);
    }
}