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
    public class WriteJsonTests
    {
        [Fact]
        public async Task WriteJson()
        {
            var innerVariableName = "c";
            var variableName = "test";
            var value = 1;

            var complexSymbol = new DebugSymbol() {Name = variableName};
            complexSymbol.DataType = new DebugType()
            {
                Name = variableName,
                ManagedType = null,
                Members = new MemberCollection(new List<IMember>()
                {
                    new DebugSubItem()
                    {
                        SubItemName = innerVariableName,
                        Attributes = new ReadOnlyTypeAttributeCollection(new TypeAttributeCollection(new List<ITypeAttribute>() {new DebugAttribute("json", innerVariableName)}))
                    }
                })
            };

            var childSymbol = new DebugSymbol() {Name = variableName + "." + innerVariableName};
            childSymbol.DataType = new DebugType() {ManagedType = typeof(int), Name = innerVariableName};

            var clientMock = new Mock<IAdsSymbolicAccess>();

            clientMock.Setup(client => client.ReadSymbol(It.Is<string>(s => s.Equals(variableName))))
                .Returns(complexSymbol);

            clientMock.Setup(client => client.ReadSymbol(It.Is<string>(s => s.StartsWith(variableName + "." + innerVariableName))))
                .Returns(childSymbol);

            var json = new JObject();
            json.Add(innerVariableName, value);
            await clientMock.Object.WriteJson(variableName, json);
            clientMock.Verify(client => client.WriteValue(childSymbol, value), Times.Once);
        }

        [Fact]
        public async Task WriteComplexJsonArray()
        {
            var secondInnerVariableName = "a";
            var innerVariableName = "c";
            var variableName = "test";
            var originName = "o";
            var value = 1;

            var origin = new DebugSymbol();
            origin.DataType = new DebugType()
            {
                ManagedType = null,
                Members = new MemberCollection(new List<IMember>()
                {
                    new DebugSubItem()
                    {
                        SubItemName = variableName,
                        Attributes = new ReadOnlyTypeAttributeCollection(new TypeAttributeCollection(new List<ITypeAttribute>() {new DebugAttribute("json", variableName)}))
                    }
                })
            };
            var arraySymbol = new DebugSymbol();
            var elementCount = 3;
            arraySymbol.DataType = new DebugType() {Category = DataTypeCategory.Array, BaseType = new DebugType() {ManagedType = null}, Dimensions = new ReadOnlyDimensionCollection(new DimensionCollection(new List<Dimension>() {new Dimension(0, elementCount)}))};


            var complexSymbol = new DebugSymbol() {Name = variableName};
            complexSymbol.DataType = new DebugType()
            {
                ManagedType = null,
                Members = new MemberCollection(new List<IMember>()
                {
                    new DebugSubItem()
                    {
                        SubItemName = innerVariableName,
                        Attributes = new ReadOnlyTypeAttributeCollection(new TypeAttributeCollection(new List<ITypeAttribute>() {new DebugAttribute("json", innerVariableName)}))
                    }
                })
            };

            var innerComplexSymbol = new DebugSymbol() {Name = variableName + "." + innerVariableName};
            innerComplexSymbol.DataType = new DebugType()
            {
                ManagedType = null,
                Members = new MemberCollection(new List<IMember>()
                {
                    new DebugSubItem()
                    {
                        SubItemName = secondInnerVariableName,
                        Attributes = new ReadOnlyTypeAttributeCollection(new TypeAttributeCollection(new List<ITypeAttribute>() {new DebugAttribute("json", secondInnerVariableName)}))
                    }
                })
            };

            var childSymbol = new DebugSymbol() {Name = variableName + "." + innerVariableName + "." + secondInnerVariableName};
            childSymbol.DataType = new DebugType() {ManagedType = typeof(int)};

            var clientMock = new Mock<IAdsSymbolicAccess>();

            clientMock.Setup(client => client.ReadSymbol(It.Is<string>(s => s.Equals(originName))))
                .Returns(origin);

            clientMock.Setup(client => client.ReadSymbol(It.Is<string>(s => s.StartsWith(originName + "." + variableName))))
                .Returns(arraySymbol);

            clientMock.Setup(client => client.ReadSymbol(It.Is<string>(s => s.StartsWith(originName + "." + variableName + "[") && s.EndsWith("]"))))
                .Returns(complexSymbol);

            clientMock.Setup(client => client.ReadSymbol(It.Is<string>(s => s.EndsWith("]." + innerVariableName))))
                .Returns(innerComplexSymbol);

            clientMock.Setup(client => client.ReadSymbol(It.Is<string>(s => s.EndsWith("]." + innerVariableName + "." + secondInnerVariableName))))
                .Callback((string s) => { childSymbol.Name = s; })
                .Returns(childSymbol);

            var json = new JObject();
            var complex = new JObject();
            var child = new JObject();
            child.Add(secondInnerVariableName, value);
            complex.Add(innerVariableName, child);
            json.Add(variableName, new JArray(Enumerable.Repeat(complex, elementCount)));

            await clientMock.Object.WriteJson(originName, json);

            clientMock.Verify(client => client.WriteValue(childSymbol, value), Times.Exactly(elementCount));
        }

        [Fact]
        public async Task WriteJsonArray()
        {
            var variableName = "test";
            var originName = "o";
            var value = 1;

            var origin = new DebugSymbol();
            origin.DataType = new DebugType()
            {
                ManagedType = null,
                Members = new MemberCollection(new List<IMember>()
                {
                    new DebugSubItem()
                    {
                        SubItemName = variableName,
                        Attributes = new ReadOnlyTypeAttributeCollection(new TypeAttributeCollection(new List<ITypeAttribute>() {new DebugAttribute("json", variableName)}))
                    }
                })
            };
            var arraySymbol = new DebugSymbol();
            var elementCount = 3;
            arraySymbol.DataType = new DebugType() {Category = DataTypeCategory.Array, BaseType = new DebugType() {ManagedType = typeof(int)}, Dimensions = new ReadOnlyDimensionCollection(new DimensionCollection(new List<Dimension>() {new Dimension(0, elementCount)}))};


            var childSymbol = new DebugSymbol();
            childSymbol.DataType = new DebugType() {ManagedType = typeof(int)};

            var clientMock = new Mock<IAdsSymbolicAccess>();

            clientMock.Setup(client => client.ReadSymbol(It.Is<string>(s => s.Equals(originName))))
                .Returns(origin);

            clientMock.Setup(client => client.ReadSymbol(It.Is<string>(s => s.StartsWith(originName + "." + variableName))))
                .Returns(arraySymbol);

            clientMock.Setup(client => client.ReadSymbol(It.Is<string>(s => s.StartsWith(originName + "." + variableName + "[") && s.EndsWith("]"))))
                .Returns(childSymbol);


            var json = new JObject();
            json.Add(variableName, new JArray(Enumerable.Repeat(value, elementCount)));

            await clientMock.Object.WriteJson(originName, json);

            clientMock.Verify(client => client.WriteValue(childSymbol, value), Times.Exactly(elementCount));
        }


    }


}