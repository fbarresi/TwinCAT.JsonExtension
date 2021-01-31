using System;
using System.Text;
using TwinCAT.Ads;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;

namespace TwinCAT.JsonExtension.Tests
{
    public class DebugSubItem : IAttributedInstance, IMember
    {
        public int Size { get; }
        public bool IsBitType { get; }
        public int BitSize { get; }
        public int ByteSize { get; }
        public bool IsByteAligned { get; }
        public IDataType? DataType { get; set; }
        public string TypeName { get; }
        public string InstanceName { get; set; }
        public string InstancePath { get; }
        public bool IsStatic { get; }
        public bool IsReference { get; }
        public bool IsPointer { get; }
        public string Comment { get; }
        public ITypeAttributeCollection Attributes { get; set; }
        public Encoding ValueEncoding { get; }
        public string Name { get; set; }
        public string SubItemName { get; set; }
        public IDataType ParentType { get; }
        public int Offset { get; }
        public int ByteOffset { get; }
        public int BitOffset { get; }
    }
}