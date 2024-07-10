using Umbraco13Demo.Mapper;

namespace Umbraco13Demo.POCOs
{
    public class MyDocumentTemp : BaseUmbTemplate
    {
        public DateTime MyDateTime { get; set; }
        public string MyTextArea { get; set; }

        [UmbAlias("MyTextBox")]
        public string YourTextBox { get; set; }
        public bool MyTrueFalse { get; set; }
        public IEnumerable<string> MyCheckBoxList { get; set; }

        public IEnumerable<string> MyTags { get; set; }

        /// <summary>
        /// MediaUrl
        /// </summary>
        public string MyMediaPicker { get; set; }

    }
}
