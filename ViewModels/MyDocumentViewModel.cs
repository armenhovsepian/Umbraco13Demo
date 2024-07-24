using System.Runtime.Serialization;
using Umbraco13Demo.Mapper;

namespace Umbraco13Demo.ViewModels
{
    //[UmbContentType("myDocument")]
    [DataContract(Name = "myDocument")]
    public class MyDocumentViewModel
    {
        [UmbAlias("MyCheckBoxList")]
        public IEnumerable<string> CheckBoxList { get; set; }
        public DateTime MyDateTime { get; set; }
        public IEnumerable<string> MyDropDown { get; set; }
        public string MyTinyMce { get; set; }

        //[UmbContentType("Partners")]
        public IEnumerable<PartnerViewModel> Partners { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
