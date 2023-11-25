using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using TwinCAT.JsonService.Services;
using TwinCAT.JsonService.Settings;

namespace TwinCAT.JsonService.Tests;

[TestFixture]
public class ClientServiceTests
{
    private Mock<ILogger<ClientService>> Logger;

    [SetUp]
    public void Setup()
    {
        Logger = new Mock<ILogger<ClientService>>();
    }

    [Test]
    public void TestConstructor()
    {
        var clientService = new ClientService(Logger.Object, new BeckhoffClientSettings());
        clientService.ShouldNotBeNull();
    }
    
    [Test]
    public void TestConstructorWithNullParameters()
    {
        var clientService = new ClientService(null, null);
        clientService.ShouldNotBeNull();
    }
}