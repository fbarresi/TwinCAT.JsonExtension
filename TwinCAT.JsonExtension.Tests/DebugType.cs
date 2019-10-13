using System;
using System.Linq;
using TwinCAT.Ads;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;

namespace TwinCAT.JsonExtension.Tests
{
    internal class DebugType : ITcAdsDataType
    {
        public int Size { get; set;}
        public bool IsBitType { get; set;}
        public int BitSize { get; set;}
        public int ByteSize { get; set;}
        public bool IsByteAligned { get; set;}
        public int Id { get; set;}
        public DataTypeCategory Category { get; set;}
        public string Name { get; set;}
        public string Namespace { get; set;}
        public string FullName { get; set;}
        public bool IsPrimitive { get; set;}
        public bool IsContainer { get; set;}
        public bool IsPointer { get; set;}
        public bool IsReference { get; set;}
        public ReadOnlyTypeAttributeCollection Attributes { get; set;}
        public string Comment { get; set;}
        public IDataType ResolveType(DataTypeResolveStrategy type)
        {
            return EnumInfos as IDataType;
        }

        public AdsDatatypeId DataTypeId { get; set;}
        public bool HasArrayInfo { get; set;}
        public ReadOnlyDimensionCollection Dimensions { get; set;}
        public bool HasRpcMethods { get; set;}
        public ReadOnlyRpcMethodCollection RpcMethods { get; set;}
        public ITcAdsDataType BaseType { get; set;}
        public string BaseTypeName { get; set;}
        public bool HasEnumInfo { get; set;}
        public ReadOnlyEnumValueCollection EnumInfos { get; set;}
        public ReadOnlyEnumValueCollection EnumValues { get; set;}
        public ReadOnlySubItemCollection SubItems { get; set;}
        public bool HasSubItemInfo { get; set;}
        public bool IsEnum { get; set;}
        public bool IsArray { get; set;}
        public bool IsStruct { get; set;}
        public bool IsSubItem { get; set;}
        public bool IsAlias { get; set;}
        public bool IsString { get; set;}
        public Type ManagedType { get; set;}
        public bool IsOversamplingArray { get; set;}
        public AdsDataTypeFlags Flags { get; set;}
        public bool IsJaggedArray { get; set;}
    }
}