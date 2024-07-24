using Umbraco.Cms.Core.Models;
using Umbraco13Demo.Mapper;

namespace Umbraco13Demo.ViewModels
{
    [UmbContentType("Partner")]
    public class PartnerViewModel
    {
        public string PartnerName { get; set; }
        public MediaWithCrops PartnerLogo { get; set; }

    }
}
