namespace Umbraco13Demo.Mapper
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class UmbAlias : Attribute
    {
        public string Alias { get; private set; }
        public UmbAlias(string alias) => Alias = alias;
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
