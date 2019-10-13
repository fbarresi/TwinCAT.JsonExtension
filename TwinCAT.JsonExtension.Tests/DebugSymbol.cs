using System;
using System.Collections.Generic;
using TwinCAT.Ads;
using TwinCAT.TypeSystem;

namespace TwinCAT.JsonExtension.Tests
{
    internal class DebugSymbol : ITcAdsSymbol, ITcAdsSymbol5
    {
        public long IndexGroup { get; set; }
        public long IndexOffset { get; set; }
        public int Size { get; set; }
        public AdsDatatypeId Datatype { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Comment { get; set; }
        public bool IsPersistent { get; set;}
        public bool IsBitType { get; set;}
        public bool IsReference { get; set;}
        public bool IsPointer { get; set;}
        public bool IsTypeGuid { get; set;}
        public bool IsReadOnly { get; set;}
        public bool IsTcComInterfacePointer { get; set;}
        public int ContextMask { get; set;}
        public bool IsArray { get; set;}
        public int ArrayDimensions { get; set;}
        public AdsDatatypeArrayInfo[] ArrayInfos { get; set;}
        public ReadOnlyTypeAttributeCollection Attributes { get; set;}
        public bool IsEnum { get; set;}
        public bool IsStruct { get; set;}
        public bool HasRpcMethods { get; set;}
        public ReadOnlyRpcMethodCollection RpcMethods { get; set;}
        public DataTypeCategory Category { get; set;}
        public int BitSize { get; set;}
        public int ByteSize { get; set;}
        public bool IsRecursive(IEnumerable<ITcAdsSymbol5> parents)
        {
            throw new System.NotImplementedException();
        }

        public ITcAdsDataType DataType { get; set;}
        public AdsDatatypeId DataTypeId { get; set;}
        public string TypeName { get; set;}
        public bool IsStatic { get; set;}
    }
}