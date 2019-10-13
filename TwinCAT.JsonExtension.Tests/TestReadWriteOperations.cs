using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Shouldly;
using TwinCAT.Ads;
using Xunit;

namespace TwinCAT.JsonExtension.Tests
{
    public class TestReadWriteOperations
    {
        private Mock<IAdsSymbolicAccess> GetClientMock<T>(ITcAdsSymbol symbol, T value)
        {
            var clientMock = new Mock<IAdsSymbolicAccess>();

            clientMock.Setup(client => client.ReadSymbol(It.IsAny<ITcAdsSymbol>()))
                .Returns(value);

            clientMock.Setup(client => client.ReadSymbolInfo(It.IsAny<string>()))
                .Returns(symbol);

            return clientMock;
        }

        [Fact]
        public async Task ReadSymbolWithDifferentType()
        {
            int value = 10;
            var symbol = new DebugSymbol();
            var variableName = "test.object";

            var clientMock = GetClientMock(symbol, value);

            var readVariable = await clientMock.Object.ReadAsync<string>(variableName);

            clientMock.Verify(client => client.ReadSymbolInfo(variableName), Times.Once);
            clientMock.Verify(client => client.ReadSymbol(symbol), Times.Once);
            readVariable.ShouldBe(value.ToString());
        }

        [Fact]
        public async Task ReadSymbol()
        {
            int value = 11;
            var symbol = new DebugSymbol();
            var variableName = "test.object2";

            var clientMock = GetClientMock(symbol, value);
            
            var readVariable = await clientMock.Object.ReadAsync<int>(variableName);

            clientMock.Verify(client => client.ReadSymbolInfo(variableName), Times.Once);
            clientMock.Verify(client => client.ReadSymbol(symbol), Times.Once);
            readVariable.ShouldBe(value);
        }

        [Fact]
        public async Task WriteSymbol()
        {
            var symbol = new DebugSymbol();
            var targetType = typeof(int);
            symbol.DataType = new DebugType { ManagedType = targetType };
            int value = 12;
            var variableName = "test.object3";

            var clientMock = GetClientMock(symbol, value);

            await clientMock.Object.WriteAsync(variableName, value);

            clientMock.Verify(client => client.ReadSymbolInfo(variableName), Times.Once);
            clientMock.Verify(client => client.WriteSymbol(symbol, value), Times.Once);
        }

        [Fact]
        public async Task WriteSymbolWithDifferentType()
        {
            var symbol = new DebugSymbol();
            var targetType = typeof(string);
            symbol.DataType = new DebugType { ManagedType = targetType };
            var variableName = "test.object4";
            int value = 14;

            var clientMock = GetClientMock(symbol, value);

            await clientMock.Object.WriteAsync(variableName, value);

            clientMock.Verify(client => client.ReadSymbolInfo(variableName), Times.Once);
            clientMock.Verify(client => client.WriteSymbol(symbol, value.ToString()), Times.Once);
        }
    }
}