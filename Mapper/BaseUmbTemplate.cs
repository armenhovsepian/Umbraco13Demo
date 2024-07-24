namespace Umbraco13Demo.Mapper
{
    public class PublishedContentModel
    {
        [UmbAlias("Id")]
        public int ContentId { get; set; }

        [UmbAlias("Name")]
        public string ContentName { get; set; }
    }
}
