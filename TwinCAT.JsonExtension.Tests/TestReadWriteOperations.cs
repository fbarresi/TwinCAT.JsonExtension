using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Shouldly;
using TwinCAT.Ads;

namespace TwinCAT.JsonExtension.Tests
{
    public class TestReadWriteOperations
    {
        [Test]
        public async Task ReadSymbolWithDifferentType()
        {
            var clientMock = new Mock<IAdsSymbolicAccess>();

            var value = 10;
            clientMock.Setup(client => client.ReadSymbol(It.IsAny<ITcAdsSymbol>()))
                .Returns(value);
            var symbol = new DebugSymbol();
            clientMock.Setup(client => client.ReadSymbolInfo(It.IsAny<string>()))
                .Returns(symbol);

            var variableName = "test.object";
            var readVariable = await clientMock.Object.ReadAsync<string>(variableName);

            clientMock.Verify(client => client.ReadSymbolInfo(variableName), Times.Once);
            clientMock.Verify(client => client.ReadSymbol(symbol), Times.Once);
            
            readVariable.ShouldBe(value.ToString());
        }

        [Test]
        public async Task ReadSymbol()
        {
            var clientMock = new Mock<IAdsSymbolicAccess>();

            int value = 10;
            clientMock.Setup(client => client.ReadSymbol(It.IsAny<ITcAdsSymbol>()))
                .Returns(value);
            var symbol = new DebugSymbol();
            clientMock.Setup(client => client.ReadSymbolInfo(It.IsAny<string>()))
                .Returns(symbol);

            var variableName = "test.object";
            var readVariable = await clientMock.Object.ReadAsync<int>(variableName);

            clientMock.Verify(client => client.ReadSymbolInfo(variableName), Times.Once);
            clientMock.Verify(client => client.ReadSymbol(symbol), Times.Once);

            readVariable.ShouldBe(value);
        }

        [Test]
        public async Task WriteSymbol()
        {
            var clientMock = new Mock<IAdsSymbolicAccess>();

            var symbol = new DebugSymbol();
            var targetType = typeof(int);
            symbol.DataType = new DebugType(){ManagedType = targetType};
            
            clientMock.Setup(client => client.ReadSymbolInfo(It.IsAny<string>()))
                .Returns(symbol);
            var variableName = "test.object";
            int value = 10;
            await clientMock.Object.WriteAsync(variableName, value);

            clientMock.Verify(client => client.ReadSymbolInfo(variableName), Times.Once);
            clientMock.Verify(client => client.WriteSymbol(symbol, value), Times.Once);
        }

        [Test]
        public async Task WriteSymbolWithDifferentType()
        {
            var clientMock = new Mock<IAdsSymbolicAccess>();

            var symbol = new DebugSymbol();
            var targetType = typeof(string);
            symbol.DataType = new DebugType { ManagedType = targetType };

            clientMock.Setup(client => client.ReadSymbolInfo(It.IsAny<string>()))
                .Returns(symbol);
            var variableName = "test.object";
            int value = 10;
            await clientMock.Object.WriteAsync(variableName, value);

            clientMock.Verify(client => client.ReadSymbolInfo(variableName), Times.Once);
            clientMock.Verify(client => client.WriteSymbol(symbol, value.ToString()), Times.Once);
        }
    }
}