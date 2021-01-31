using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads;
using TwinCAT.Ads.Internal;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;

namespace TwinCAT.JsonExtension.Tests
{
    internal class DebugType : IDataType, IStructType, IBitSize, IManagedMappableType, IResolvableType, IBindable2, IBindable, IArrayType
    {
        public int Size { get; }
        public bool IsBitType { get; }
        public int BitSize { get; }
        public int ByteSize { get; }
        public bool IsByteAligned { get; }
        public int Id { get; }
        public DataTypeCategory Category { get; set; }
        public string Name { get; set; }
        public string Namespace { get; }
        public string FullName { get; }
        public bool IsPrimitive { get; }
        public bool IsContainer { get; }
        public bool IsPointer { get; }
        public bool IsReference { get; }
        public ITypeAttributeCollection Attributes { get; }
        public string Comment { get; }
        public Type ManagedType { get; set; }
        public IMemberCollection Members { get; set; }
        public string BaseTypeName { get; }
        IDataType? IStructType.BaseType { get; }
        public IMemberCollection AllMembers { get; }
        public bool HasRpcMethods { get; }
        public DebugType BaseType { get; set; }
        public IDataType ResolveType(DataTypeResolveStrategy type)
        {
            throw new NotImplementedException();
        }

        public void Bind(IBinder binder)
        {
            throw new NotImplementedException();
        }

        public bool IsBound { get; }
        public bool IsBindingResolved(bool recurse)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResolveWithBinderAsync(bool recurse, IBinder binder, CancellationToken cancel)
        {
            throw new NotImplementedException();
        }

        public bool ResolveWithBinder(bool recurse, IBinder binder)
        {
            throw new NotImplementedException();
        }

        public IDimensionCollection Dimensions { get; set; }
        public IDataType? ElementType { get; }
        public string ElementTypeName { get; }
        public bool IsJagged { get; }
        public int JaggedLevel { get; }

        public static Type t = typeof(bool);

    }
}