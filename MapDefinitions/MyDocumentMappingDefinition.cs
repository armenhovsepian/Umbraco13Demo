using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection;
using System.Runtime.Serialization;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco13Demo.ViewModels;

namespace Umbraco13Demo.MapDefinitions
{
    public class MyDocumentMappingDefinition : IMapDefinition
    {

        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define<MyDocument, MyDocumentViewModel>((source, context) => new MyDocumentViewModel(), Map);
            //mapper.Define<MyDocument, MyDocumentDto>();
        }

        private void Map(MyDocument source, MyDocumentViewModel target, MapperContext context)
        {
            target = BaseMap(source, target);

            // Other mappings

        }

        private TTarget BaseMap<TSource, TTarget>(TSource source, TTarget target) where TSource : Umbraco.Cms.Core.Models.PublishedContent.PublishedContentModel
        {
            foreach (PropertyInfo targetPropertyInfo in typeof(TTarget).GetRuntimeProperties())
            {
                foreach (PropertyInfo sourcePropertyInfo in source.GetType().GetProperties())
                {
                    if (targetPropertyInfo.Name == sourcePropertyInfo.Name || targetPropertyInfo.GetCustomAttribute<DataMemberAttribute>()?.Name == sourcePropertyInfo.Name)
                    {
                        // CLR data types
                        if (targetPropertyInfo.PropertyType == sourcePropertyInfo.PropertyType)
                        {
                            targetPropertyInfo.SetValue(target, sourcePropertyInfo.GetValue(source));
                            continue;
                        }

                        // tinyMCE
                        else if (sourcePropertyInfo.PropertyType == typeof(IHtmlEncodedString))
                        {
                            var value = sourcePropertyInfo.GetValue(source);
                            targetPropertyInfo.SetValue(target, value.ToString());
                            continue;
                        }
                    }
                }
            }

            return target;
        }
    }



    public class MappingComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.WithCollectionBuilder<MapDefinitionCollectionBuilder>().Add<MyDocumentMappingDefinition>();
            builder.WithCollectionBuilder<MapDefinitionCollectionBuilder>().Add<MyDocumentContentMappingDefinition>();
        }
    }
}
