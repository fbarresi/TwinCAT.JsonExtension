using System;
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
    public class WriteArrayTest
    {
        [Fact]
        public async Task WriteJsonArray()
        {
            var originName = "o";
            var value = 1;

            var arraySymbol = new DebugSymbol();
            var elementCount = 3;
            arraySymbol.DataType = new DebugType()
            {
                Category = DataTypeCategory.Array, 
                BaseType = new DebugType() {ManagedType = typeof(int)}, Dimensions = new ReadOnlyDimensionCollection(new DimensionCollection(new List<Dimension>() {new Dimension(0, elementCount)})),
                ElementType = new DebugType() {ManagedType = typeof(int)}
            };


            var childSymbol = new DebugSymbol();
            childSymbol.DataType = new DebugType()
            {
                ManagedType = typeof(int), 
                Category = DataTypeCategory.Primitive
            };

            var clientMock = new Mock<IAdsSymbolicAccess>();

            clientMock.Setup(client => client.ReadSymbol(It.Is<string>(s => s.StartsWith(originName))))
                .Returns(arraySymbol);

            clientMock.Setup(client => client.ReadSymbol(It.Is<string>(s => s.StartsWith(originName  + "[") && s.EndsWith("]"))))
                .Returns(childSymbol);


            var array = new JArray(Enumerable.Repeat(value, elementCount));

            await clientMock.Object.WriteJson(originName, array);

            clientMock.Verify(client => client.WriteValue(childSymbol, It.Is<object>(v => int.Parse(v.ToString()) == value)), Times.Exactly(elementCount));
        }
        
        [Fact]
        public async Task WriteWrongVariableForJsonArray()
        {
            var originName = "o";
            var value = 1;

            var arraySymbol = new DebugSymbol();
            var elementCount = 3;
            arraySymbol.DataType = new DebugType() {Category = DataTypeCategory.Array, BaseType = new DebugType() {ManagedType = typeof(int)}, Dimensions = new ReadOnlyDimensionCollection(new DimensionCollection(new List<Dimension>() {new Dimension(0, elementCount)}))};


            var childSymbol = new DebugSymbol();
            childSymbol.DataType = new DebugType() {ManagedType = typeof(int)};

            var clientMock = new Mock<IAdsSymbolicAccess>();

            clientMock.Setup(client => client.ReadSymbol(It.Is<string>(s => s.StartsWith(originName))))
                .Returns(arraySymbol);

            clientMock.Setup(client => client.ReadSymbol(It.Is<string>(s => s.StartsWith(originName  + "[") && s.EndsWith("]"))))
                .Returns(childSymbol);


            var array = new JArray(Enumerable.Repeat(value, elementCount));

            await clientMock.Object.WriteJson(originName+"[0]", array).ShouldThrowAsync(typeof(InvalidOperationException));

            clientMock.Verify(client => client.WriteValue(childSymbol, value), Times.Never);
        }
        
        [Fact]
        public async Task WriteComplexJsonArray()
        {
            var secondInnerVariableName = "a";
            var innerVariableName = "c";
            var variableName = "test";
            var value = 1;

            
            var arraySymbol = new DebugSymbol();
            var elementCount = 3;
            arraySymbol.DataType = new DebugType()
            {
                Category = DataTypeCategory.Array, 
                BaseType = new DebugType() {ManagedType = null}, 
                Dimensions = new ReadOnlyDimensionCollection(new DimensionCollection(new List<Dimension>() {new Dimension(0, elementCount)})),
                ElementType = new DebugType(){ Category = DataTypeCategory.Struct}
            };


            var complexSymbol = new DebugSymbol() {Name = variableName};
            complexSymbol.DataType = new DebugType()
            {
                ManagedType = null,
                Members = new MemberCollection(new List<IMember>()
                {
                    new DebugSubItem()
                    {
                        InstanceName = innerVariableName,
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
                        InstanceName = secondInnerVariableName,
                        SubItemName = secondInnerVariableName,
                        Attributes = new ReadOnlyTypeAttributeCollection(new TypeAttributeCollection(new List<ITypeAttribute>() {new DebugAttribute("json", secondInnerVariableName)}))
                    }
                })
            };

            var childSymbol = new DebugSymbol() {Name = variableName + "." + innerVariableName + "." + secondInnerVariableName};
            childSymbol.DataType = new DebugType() {ManagedType = typeof(int)};

            var clientMock = new Mock<IAdsSymbolicAccess>();

            clientMock.Setup(client => client.ReadSymbol(It.Is<string>(s => s.StartsWith(variableName))))
                .Returns(arraySymbol);

            clientMock.Setup(client => client.ReadSymbol(It.Is<string>(s => s.StartsWith(variableName + "[") && s.EndsWith("]"))))
                .Returns(complexSymbol);

            clientMock.Setup(client => client.ReadSymbol(It.Is<string>(s => s.EndsWith("]." + innerVariableName))))
                .Returns(innerComplexSymbol);

            clientMock.Setup(client => client.ReadSymbol(It.Is<string>(s => s.EndsWith("]." + innerVariableName + "." + secondInnerVariableName))))
                .Callback((string s) => { childSymbol.Name = s; })
                .Returns(childSymbol);


            var complex = new JObject();
            var child = new JObject();
            child.Add(secondInnerVariableName, value);
            complex.Add(innerVariableName, child);
            var array = new JArray(Enumerable.Repeat(complex, elementCount));

            await clientMock.Object.WriteJson(variableName, array);

            clientMock.Verify(client => client.WriteValue(childSymbol, It.Is<object>(v => int.Parse(v.ToString()) == value)), Times.Exactly(elementCount));

        }
    }
}