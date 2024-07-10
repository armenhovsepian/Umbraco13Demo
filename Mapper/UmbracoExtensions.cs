using System.Reflection;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco13Demo.Mapper.UmbracoEditors;

namespace Umbraco13Demo.Mapper
{
    public static class UmbracoExtensions
    {
        public static T As<T>(this IPublishedContent content)
        {
            // Create an empty instance of the POCO
            var poco = Activator.CreateInstance<T>();

            // Discover properties of the poco with reflection
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var pocoType = poco.GetType();

            var infos = typeof(T).GetTypeInfo();

            //var viewModelProperties = TypeDescriptor.GetProperties(typeof(T));

            if (properties.Any(p => p.Name.Equals("Id")))
            {
                pocoType.GetProperty("Id").SetValue(poco, content.Id, null);
            }

            if (properties.Any(p => p.Name.Equals("Name")))
            {
                pocoType.GetProperty("Name").SetValue(poco, content.Name, null);
            }


            foreach (PropertyInfo pocoPropertyInfo in properties.Where(p => p.CanRead && p.CanWrite))
            {
                //var t = pocoPropertyInfo.PropertyType == typeof(string);

                //foreach (var property in content.Properties)
                //{
                //    var alias = property.Alias;
                //    var editorAlias = property.PropertyType.EditorAlias;
                //    var editorClrType = property.PropertyType.ClrType;
                //}

                var alias = string.Empty;
                if (content.HasProperty(pocoPropertyInfo.Name))
                {
                    alias = pocoPropertyInfo.Name;
                }
                else
                {
                    // option 1
                    var att = pocoPropertyInfo.GetCustomAttribute<UmbAlias>();
                    if (att == null || !content.HasProperty(att.Alias))
                    {
                        continue;
                    }

                    alias = att.Alias;

                    // option 2
                    //UmbAlias att = (UmbAlias)Attribute.GetCustomAttribute(pocoPropertyInfo, typeof(UmbAlias));
                    //if (att == null || !content.HasProperty(att.Alias))
                    //{
                    //    continue;
                    //}
                    //alias = att.Alias;
                }



                if (content.HasProperty(alias))
                {
                    var t = pocoPropertyInfo.PropertyType.GetType();
                    var value = content.Value(alias);

                    var contentProperty = content.GetProperty(alias);

                    if (contentProperty.HasValue())
                    {
                        var propertyValue = contentProperty.GetValue();


                        var res = contentProperty.PropertyType.ModelClrType.Assembly.FullName == pocoPropertyInfo.PropertyType.Assembly.FullName;

                        if (contentProperty.PropertyType.ClrType == pocoPropertyInfo.PropertyType)
                        {
                            pocoType.GetProperty(pocoPropertyInfo.Name).SetValue(poco, propertyValue, null);
                        }
                        else
                        {
                            switch (contentProperty.PropertyType.EditorAlias)
                            {
                                case "Umbraco.TextBox":
                                case "Umbraco.TextArea":
                                case "Umbraco.TinyMCE":
                                    pocoType.GetProperty(pocoPropertyInfo.Name).SetValue(poco, propertyValue.ToString(), null);
                                    break;
                                case "Umbraco.MediaPicker":

                                    var interfacesThatTypeImplements = pocoPropertyInfo.PropertyType.GetInterfaces()
                                            .Where(iType => iType.Name == "IMediaPickerUmbEditor");

                                    var isInstanceOfType = pocoPropertyInfo.PropertyType.IsInstanceOfType(typeof(MediaPickerUmbEditor));

                                    // This property editors returns a single MediaWithCrops item if the "Pick multiple items"
                                    // Data Type setting is disabled or a collection if it is enabled.

                                    var typedMediaPicker = content.Value<MediaWithCrops>(alias);
                                    typedMediaPicker.MediaUrl();

                                    foreach (var entry in content.Value<IEnumerable<MediaWithCrops>>(alias))
                                    {
                                        var src = entry.MediaUrl();
                                    }

                                    if (pocoPropertyInfo.PropertyType.BaseType == typeof(MediaPickerUmbEditor))
                                    {
                                        var url = content.Url();

                                        var mediaPicker = Activator.CreateInstance(pocoPropertyInfo.PropertyType);
                                        mediaPicker.GetType().GetProperty("Url").SetValue(mediaPicker, "https://example.com");
                                        pocoType.GetProperty(pocoPropertyInfo.Name).SetValue(poco, mediaPicker, null);
                                    }

                                    break;
                                case "Umbraco.TrueFalse":
                                    break;
                                case "Umbraco.DropDown.Flexible":
                                    break;
                                case "Umbraco.ContentPicker":
                                    break;
                                case "Umbraco.MultiNodeTreePicker":
                                    break;
                                case "Umbraco.Tags":
                                    pocoType.GetProperty(pocoPropertyInfo.Name).SetValue(poco, content.Value<IEnumerable<string>>(alias), null);
                                    break;
                            }
                        }

                    }
                }
            }

            return poco;
        }
    }
}

