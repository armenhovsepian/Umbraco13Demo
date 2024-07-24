using Umbraco13Demo.Mapper;

namespace Umbraco13Demo.POCOs
{
    public class MyDocumentTemp : PublishedContentModel
    {
        public MyDocumentTemp()
        {
            MyCheckBoxList = Enumerable.Empty<string>().ToList();
            MyTags = Enumerable.Empty<string>().ToList();
            MyMediaPicker = Enumerable.Empty<string>().ToList();
        }

        public DateTime MyDateTime { get; set; }
        public string MyTextArea { get; set; }

        [UmbAlias("MyTextBox")]
        public string YourTextBox { get; set; }
        public bool MyTrueFalse { get; set; }
        public List<string> MyCheckBoxList { get; set; }

        public List<string> MyTags { get; set; }

        public string MyDropDown { get; set; }

        /// <summary>
        /// MediaUrl
        /// </summary>
        public List<string> MyMediaPicker { get; set; }

    }
}
