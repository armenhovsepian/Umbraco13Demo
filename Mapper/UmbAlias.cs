namespace Umbraco13Demo.Mapper
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class UmbAlias : Attribute
    {
        public string Alias { get; set; }
        public UmbAlias(string alias)
        {
            Alias = alias;
        }
    }
}
