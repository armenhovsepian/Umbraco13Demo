namespace Umbraco13Demo.Mapper
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class UmbAlias : Attribute
    {
        public string Alias { get; }
        public UmbAlias(string alias) => Alias = alias;
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class UmbContentType : Attribute
    {
        public string Alias { get; }
        public UmbContentType(string alias) => Alias = alias;
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class UmbNestedContentType : Attribute
    {
        public string Alias { get; }
        public UmbNestedContentType(string alias) => Alias = alias;
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class UmbIgnore : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class UmbContentId : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class UmbContentName : Attribute
    {

    }
}
