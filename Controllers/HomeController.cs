using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco13Demo.Mapper;
using Umbraco13Demo.POCOs;

namespace Umbraco13Demo.Controllers
{
    public class HomeController : SurfaceController
    {
        private readonly UmbracoHelper _umbracoHelper;
        private readonly IContentService _contentService;
        public HomeController(IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            UmbracoHelper umbracoHelper,
            IContentService contentService)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _umbracoHelper = umbracoHelper;
            _contentService = contentService;
        }

        public IActionResult Index()
        {
            var content = _contentService.GetById(1075);

            var publishedContent = _umbracoHelper.Content(1075);

            var pTags = publishedContent.Value<IEnumerable<string>>("myTags");

            //var myDocument3 = publishedContent.Simple<MyDocumentTemp>();

            foreach (IPublishedProperty property in publishedContent.Properties)
            {
                var alias = property.Alias;
                var type = property.PropertyType.DataType.EditorAlias;
                var value = property.GetValue();

                if (value != null)
                {

                }
            }

            var myDocument = publishedContent.MapTo<MyDocumentTemp>();

            return View(myDocument);
        }
    }
}
