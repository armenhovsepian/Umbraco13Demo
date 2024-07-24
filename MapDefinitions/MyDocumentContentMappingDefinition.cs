using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco13Demo.ViewModels;

namespace Umbraco13Demo.MapDefinitions
{
    public class MyDocumentContentMappingDefinition : IMapDefinition
    {
        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define<IPublishedContent, MyDocumentViewModel>((source, context) => new MyDocumentViewModel(), Map);
        }

        private void Map(IPublishedContent content, MyDocumentViewModel model, MapperContext context)
        {
            model.Name = content.Name;
        }
    }
}
