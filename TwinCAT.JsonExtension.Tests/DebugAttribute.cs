using TwinCAT.TypeSystem;

namespace TwinCAT.JsonExtension.Tests
{
    public class DebugAttribute: ITypeAttribute
    {
        public DebugAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }
    }
}