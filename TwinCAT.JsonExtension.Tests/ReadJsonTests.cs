using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json.Linq;
using Shouldly;
using TwinCAT.Ads;
using TwinCAT.TypeSystem;
using Xunit;

namespace TwinCAT.JsonExtension.Tests
{
    public class ReadJsonTests
    {
        [Fact]
        public async Task TestReadSimpleJson()
        {
            var tcAdsSymbol = new DebugSymbol();
            tcAdsSymbol.DataType = new DebugType(){Category = DataTypeCategory.Primitive, ManagedType = typeof(int)};
            var value = 1;
            var clientMock = TestReadWriteOperations.GetClientMock(tcAdsSymbol, value);
            var variableName = "test";
            var expected = new JObject();
            expected.Add(variableName, value);
            var json = await clientMock.Object.ReadJson(variableName);
            JToken.DeepEquals(json, expected).ShouldBeTrue();
        }

        [Fact]
        public async Task TestReadArray()
        {
            var tcAdsSymbol = new DebugSymbol();
            tcAdsSymbol.DataType = new DebugType() { Category = DataTypeCategory.Array, BaseType = new DebugType(){ManagedType = typeof(int)}};
            var value = new int[]{1,2,3};
            var clientMock = TestReadWriteOperations.GetClientMock(tcAdsSymbol, value);
            var variableName = "test";
            var expected = new JObject();
            expected.Add(variableName, new JArray(value));
            var json = await clientMock.Object.ReadJson(variableName);
            JToken.DeepEquals(json, expected).ShouldBeTrue();
        }
        
        [Fact]
        public async Task TestReadMultidimensionalArray()
        {
            var tcAdsSymbol = new DebugSymbol();
            tcAdsSymbol.DataType = new DebugType() { Category = DataTypeCategory.Array, BaseType = new DebugType(){ManagedType = typeof(int)}};
            var value = new int[][]{new int[] {1,2,3}, new int[] {4,5,6}};
            var clientMock = TestReadWriteOperations.GetClientMock(tcAdsSymbol, value);
            var variableName = "test";
            var expected = new JObject();
            expected.Add(variableName, new JArray(new JArray(value[0]), new JArray(value[1])));
            var json = await clientMock.Object.ReadJson(variableName);
            JToken.DeepEquals(json, expected).ShouldBeTrue();
        }

        [Fact]
        public async Task TestReadComplexType()
        {
            var innerVariableName = "c";
            var variableName = "test";
            var value = 1;

            var complexSymbol = new DebugSymbol();
            complexSymbol.DataType = new DebugType(){ManagedType = null, SubItems = new ReadOnlySubItemCollection(new List<ITcAdsSubItem>(){new DebugSubItem()
            {
                SubItemName = innerVariableName,
                Attributes = new ReadOnlyTypeAttributeCollection(new TypeAttributeCollection(new List<ITypeAttribute>(){new DebugAttribute("json", innerVariableName)})) 
            }})};

            var childSymbol = new DebugSymbol();
            childSymbol.DataType = new DebugType() { ManagedType = typeof(int) };

            var clientMock = new Mock<IAdsSymbolicAccess>();

            clientMock.Setup(client => client.ReadSymbol(It.Is<ITcAdsSymbol>(s => s == childSymbol)))
                .Returns(1);

            clientMock.Setup(client => client.ReadSymbolInfo(It.Is<string>(s => s.Equals(variableName))))
                .Returns(complexSymbol);

            clientMock.Setup(client => client.ReadSymbolInfo(It.Is<string>(s => s.StartsWith(variableName+"."+ innerVariableName))))
                .Returns(childSymbol);

            var expected = new JObject();
            expected.Add(innerVariableName, value);

            var json = await clientMock.Object.ReadJson(variableName);
            JToken.DeepEquals(json, expected).ShouldBeTrue();
        }

        [Fact]
        public async Task TestReadVeryComplexType()
        {
            var secondInnerVariableName = "a";
            var innerVariableName = "c";
            var variableName = "test";
            var value = 1;

            var complexSymbol = new DebugSymbol();
            complexSymbol.DataType = new DebugType()
            {
                ManagedType = null,
                SubItems = new ReadOnlySubItemCollection(new List<ITcAdsSubItem>(){new DebugSubItem()
                {
                    SubItemName = innerVariableName,
                    Attributes = new ReadOnlyTypeAttributeCollection(new TypeAttributeCollection(new List<ITypeAttribute>(){new DebugAttribute("json", innerVariableName)}))
                }})
            };

            var innerComplexSymbol = new DebugSymbol();
            innerComplexSymbol.DataType = new DebugType()
            {
                ManagedType = null,
                SubItems = new ReadOnlySubItemCollection(new List<ITcAdsSubItem>(){new DebugSubItem()
                {
                    SubItemName = secondInnerVariableName,
                    Attributes = new ReadOnlyTypeAttributeCollection(new TypeAttributeCollection(new List<ITypeAttribute>(){new DebugAttribute("json", secondInnerVariableName) }))
                }})
            };

            var childSymbol = new DebugSymbol();
            childSymbol.DataType = new DebugType() { ManagedType = typeof(int) };

            var clientMock = new Mock<IAdsSymbolicAccess>();

            clientMock.Setup(client => client.ReadSymbol(It.Is<ITcAdsSymbol>(s => s == childSymbol)))
                .Returns(1);

            clientMock.Setup(client => client.ReadSymbolInfo(It.Is<string>(s => s.Equals(variableName))))
                .Returns(complexSymbol);
            
            clientMock.Setup(client => client.ReadSymbolInfo(It.Is<string>(s => s.StartsWith(variableName + "." + innerVariableName))))
                .Returns(innerComplexSymbol);

            clientMock.Setup(client => client.ReadSymbolInfo(It.Is<string>(s => s.StartsWith(variableName + "."+ innerVariableName+"."+secondInnerVariableName))))
                .Returns(childSymbol);

            var expected = new JObject();
            var child = new JObject();
            child.Add(secondInnerVariableName, value);
            expected.Add(innerVariableName, child);

            var json = await clientMock.Object.ReadJson(variableName);
            JToken.DeepEquals(json, expected).ShouldBeTrue();
        }

        [Fact]
        public async Task TestReadVeryComplexArray()
        {
            var arraySymbol = new DebugSymbol();
            var elementCount = 3;
            arraySymbol.DataType = new DebugType() { Category = DataTypeCategory.Array, BaseType = new DebugType() { ManagedType = null }, Dimensions = new ReadOnlyDimensionCollection(new DimensionCollection(new List<Dimension>() { new Dimension(0, elementCount) })) };

            var secondInnerVariableName = "a";
            var innerVariableName = "c";
            var variableName = "test";
            var value = 1;

            var complexSymbol = new DebugSymbol();
            complexSymbol.DataType = new DebugType()
            {
                ManagedType = null,
                SubItems = new ReadOnlySubItemCollection(new List<ITcAdsSubItem>(){new DebugSubItem()
                {
                    SubItemName = innerVariableName,
                    Attributes = new ReadOnlyTypeAttributeCollection(new TypeAttributeCollection(new List<ITypeAttribute>(){new DebugAttribute("json", innerVariableName)}))
                }})
            };

            var innerComplexSymbol = new DebugSymbol();
            innerComplexSymbol.DataType = new DebugType()
            {
                ManagedType = null,
                SubItems = new ReadOnlySubItemCollection(new List<ITcAdsSubItem>(){new DebugSubItem()
                {
                    SubItemName = secondInnerVariableName,
                    Attributes = new ReadOnlyTypeAttributeCollection(new TypeAttributeCollection(new List<ITypeAttribute>(){new DebugAttribute("json", secondInnerVariableName) }))
                }})
            };

            var childSymbol = new DebugSymbol();
            childSymbol.DataType = new DebugType() { ManagedType = typeof(int) };

            var clientMock = new Mock<IAdsSymbolicAccess>();

            clientMock.Setup(client => client.ReadSymbol(It.Is<ITcAdsSymbol>(s => s == childSymbol)))
                .Returns(1);

            clientMock.Setup(client => client.ReadSymbolInfo(It.Is<string>(s => s.Equals(variableName))))
                .Returns(arraySymbol);

            clientMock.Setup(client => client.ReadSymbolInfo(It.Is<string>(s => s.StartsWith(variableName+"[") && s.EndsWith("]"))))
                .Returns(complexSymbol);

            clientMock.Setup(client => client.ReadSymbolInfo(It.Is<string>(s => s.EndsWith("]." + innerVariableName))))
                .Returns(innerComplexSymbol);

            clientMock.Setup(client => client.ReadSymbolInfo(It.Is<string>(s => s.EndsWith("]." + innerVariableName + "." + secondInnerVariableName))))
                .Returns(childSymbol);

            var expected = new JObject();
            var complex = new JObject();
            var child = new JObject();
            child.Add(secondInnerVariableName, value);
            complex.Add(innerVariableName, child);
            expected.Add(variableName, new JArray(Enumerable.Repeat(complex, elementCount)));

            var json = await clientMock.Object.ReadJson(variableName);
            JToken.DeepEquals(json, expected).ShouldBeTrue();
        }
    }
}