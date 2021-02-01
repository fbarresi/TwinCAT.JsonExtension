using System.Runtime.InteropServices;
using TwinCAT.Ads;
using TwinCAT.Ads.Internal;

namespace TwinCAT.JsonExtension.Tests
{
    public class DebugPlcTypeBufferContainer
    {
        public uint EntryLength { get; set; }
        public uint Version { get; set; }
        public uint HashValue { get; set; }
        public uint TypeHashValue { get; set; }
        public uint Size { get; set; }
        public uint Offset { get; set; }
        public AdsDataTypeId BaseTypeId { get; set; }
        public AdsDataTypeFlags Flags { get; set; }
        public ushort NameLen { get; set; }
        public ushort FieldTypeNameLen { get; set; }
        public ushort CommentLen { get; set; }
        public ushort ArrayDim { get; set; }
        public ushort SubItems { get; set; }
        public string Name { get; set; }
        public string FieldTypeName { get; set; }
        public string Comment { get; set; }
        public int LowerBound { get; set; }
        public int Elements { get; set; }
    }
}