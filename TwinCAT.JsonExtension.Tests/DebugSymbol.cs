using System;
using System.Collections.Generic;
using System.Text;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;

namespace TwinCAT.JsonExtension.Tests
{
    public class DebugSymbol : IAdsSymbol
    {
        public int Size { get; }
        public bool IsBitType { get; }
        public int BitSize { get; }
        public int ByteSize { get; }
        public bool IsByteAligned { get; }
        public IDataType? DataType { get; set; }
        public string TypeName { get; }
        public string InstanceName { get; }
        public string InstancePath => Name;
        public bool IsStatic { get; }
        public bool IsReference { get; }
        public bool IsPointer { get; }
        public string Comment { get; }
        public ITypeAttributeCollection Attributes { get; }
        public Encoding ValueEncoding { get; }
        public DataTypeCategory Category { get; }
        public ISymbol? Parent { get; }
        public ISymbolCollection<ISymbol> SubSymbols { get; }
        public bool IsContainerType { get; }
        public bool IsPrimitiveType { get; }
        public bool IsPersistent { get; }
        public bool IsReadOnly { get; }
        public bool IsRecursive { get; }
        public uint IndexGroup { get; }
        public uint IndexOffset { get; }
        public bool IsVirtual { get; }
        public byte ContextMask { get; }
        public AmsAddress? ImageBaseAddress { get; }
        public AdsDataTypeId DataTypeId { get; }
        public string Name { get; set; }
    }
}