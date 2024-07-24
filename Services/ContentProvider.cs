using System.Reflection;
using System.Runtime.Serialization;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Web.Common;
using Umbraco13Demo.Mapper;

namespace Umbraco13Demo.Services
{
    public interface IContentProvider
    {
        T GetContent<T>(int id);
    }

    public class ContentProvider : IContentProvider
    {
        private const string modelsNamespace = "Umbraco.Cms.Web.Common.PublishedModels";
        private readonly UmbracoHelper _umbracoHelper;
        private readonly IUmbracoMapper _umbracoMapper;

        public ContentProvider(UmbracoHelper umbracoHelper, IUmbracoMapper umbracoMapper)
        {
            _umbracoHelper = umbracoHelper;
            _umbracoMapper = umbracoMapper;
        }

        public T GetContent<T>(int id)
        {
            var content = _umbracoHelper.Content(id);
            if (content == null)
            {
                return default;
            }

            var contentAlias = GetContentTypeAlias<T>();

            if (string.IsNullOrEmpty(contentAlias))
            {
                return default;
            }

            var instance = CreateContentInstance(contentAlias, content);
            if (instance == null)
            {
                return default;
            }

            // Create an empty instance of the POCO
            //var poco = CreatePocoInstance<T>(instance, content);
            //return poco;

            var mapped = _umbracoMapper.Map<T>(instance);
            return mapped;
        }




        private string GetContentTypeAlias<T>()
        {
            var pocoType = typeof(T);
            var umbContentType = pocoType.GetCustomAttribute<DataContractAttribute>();

            return umbContentType == null ? string.Empty : umbContentType.Name;
        }

        private object? CreateContentInstance(string contentAlias, IPublishedContent publishedContent)
        {
            try
            {
                var asmObj = Assembly.GetExecutingAssembly();
                var instance = asmObj.CreateInstance($"{modelsNamespace}.{contentAlias}", true, BindingFlags.Instance | BindingFlags.Public, null, new object[] { publishedContent, null }, null, null);
                return instance;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private TTarget CreatePocoInstance<TTarget>(object source, IPublishedContent sourceContent)
        {
            var poco = Activator.CreateInstance<TTarget>();

            foreach (PropertyInfo targetPropertyInfo in typeof(TTarget).GetRuntimeProperties())
            {
                var propertyName = targetPropertyInfo.GetCustomAttribute<UmbAlias>()?.Alias ?? targetPropertyInfo.Name;

                foreach (PropertyInfo sourcePropertyInfo in source.GetType().GetProperties())
                {
                    if (propertyName == sourcePropertyInfo.Name)
                    {

                        if (targetPropertyInfo.PropertyType == sourcePropertyInfo.PropertyType)
                        {
                            targetPropertyInfo.SetValue(poco, sourcePropertyInfo.GetValue(source));
                            continue;
                        }

                        // tinyMCE
                        else if (sourcePropertyInfo.PropertyType == typeof(IHtmlEncodedString))
                        {
                            var value = sourcePropertyInfo.GetValue(source);
                            targetPropertyInfo.SetValue(poco, value.ToString());
                            continue;
                        }

                        // Links
                        else if (sourcePropertyInfo.PropertyType == typeof(IEnumerable<Link>))
                        {
                            var links = new List<string>();
                            foreach (var entry in sourceContent.Value<IEnumerable<Link>>(propertyName))
                            {
                                links.Add(entry.Url);
                            }

                            targetPropertyInfo.SetValue(poco, links, null);
                            continue;
                        }

                        else if (sourcePropertyInfo.PropertyType == typeof(Link))
                        {
                            var link = sourceContent.Value<Link>(propertyName).Url;
                            targetPropertyInfo.SetValue(poco, link, null);
                            continue;
                        }

                        // Media Picker
                        else if (sourcePropertyInfo.PropertyType == typeof(IEnumerable<MediaWithCrops>))
                        {
                            var mediaUrls = new List<string>();
                            foreach (var entry in sourceContent.Value<IEnumerable<MediaWithCrops>>(propertyName))
                            {
                                mediaUrls.Add(entry.MediaUrl());
                            }

                            targetPropertyInfo.SetValue(poco, mediaUrls, null);
                            continue;
                        }

                        else if (sourcePropertyInfo.PropertyType == typeof(MediaWithCrops))
                        {
                            var mediaUrl = sourceContent.Value<MediaWithCrops>(propertyName).MediaUrl();
                            targetPropertyInfo.SetValue(poco, mediaUrl);
                            continue;
                        }


                        // Content Picker
                        else if (sourcePropertyInfo.PropertyType == typeof(IPublishedContent))
                        {
                            var p = targetPropertyInfo.GetCustomAttribute<UmbContentType>();
                            if (p != null)
                            {

                            }

                            var propes = targetPropertyInfo.PropertyType.GetProperties();
                            foreach (var prop in propes)
                            {
                                var value = prop.GetValue(sourcePropertyInfo.GetValue(source));
                                targetPropertyInfo.SetValue(poco, value);
                            }

                            //var newProperty = Activator.CreateInstance(targetPropertyInfo.PropertyType);

                            //var props = targetPropertyInfo.PropertyType.GetProperties();

                            //targetPropertyInfo.SetValue(poco, newProperty);
                            continue;
                        }
                        else if (sourcePropertyInfo.PropertyType == typeof(IEnumerable<IPublishedContent>))
                        {
                            continue;
                        }

                    }
                }
            }

            return poco;
        }
    }
}
