namespace Umbraco13Demo.Mapper
{
    public class BaseUmbTemplate
    {
        [UmbAlias("Id")]
        public int ContentId { get; set; }

        [UmbAlias("Name")]
        public string ContentName { get; set; }
    }
}
