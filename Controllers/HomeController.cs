using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco13Demo.Mapper;
using Umbraco13Demo.POCOs;
using Umbraco13Demo.Services;
using Umbraco13Demo.ViewModels;

namespace Umbraco13Demo.Controllers
{
    public class HomeController : SurfaceController
    {
        private readonly IUmbracoMapper _umbracoMapper;
        private readonly UmbracoHelper _umbracoHelper;
        private readonly IContentService _contentService;
        private readonly IContentProvider _contentProvider;



        public HomeController(IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            IUmbracoMapper umbracoMapper,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            UmbracoHelper umbracoHelper,
            IContentService contentService,
            IContentProvider contentProvider)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _umbracoMapper = umbracoMapper;
            _umbracoHelper = umbracoHelper;
            _contentService = contentService;
            _contentProvider = contentProvider;
        }

        public IActionResult Index()

        {
            var myModel = _contentProvider.GetContent<MyDocumentViewModel>(1075);

            var content = _contentService.GetById(1075);

            var publishedContent = _umbracoHelper.Content(1075);

            var myDoc = new Umbraco.Cms.Web.Common.PublishedModels.MyDocument(publishedContent, null);

            var mapped = _umbracoMapper.Map<MyDocumentViewModel>(myDoc);


            var pTags = publishedContent.Value<IEnumerable<string>>("myTags");

            //var myDocument3 = publishedContent.Simple<MyDocumentTemp>();

            var n = publishedContent.Map<MyDocumentViewModel>();

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



        public IActionResult Chat()
        {
            return View();
        }
    }
}
