using System.Reflection;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco13Demo.Mapper
{
    public static class UmbracoExtensions
    {
        public static T MapTo<T>(this IPublishedContent content)
        {
            // Create an empty instance of the POCO
            var poco = Activator.CreateInstance<T>(); // as ITalk // .Unwrap();

            //poco.GetType().GetProperty("Id")?.SetValue(poco, content.Id);
            //poco.GetType().GetProperty("Name")?.SetValue(poco, content.Name);

            SetContentId(poco, content);
            SetContentName(poco, content);


            //dynamic poco1 = Activator.CreateInstance<T>();
            //poco1.Id = content.Id;
            //poco1.Name = content.Name;

            // Discover properties of the poco with reflection
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo propertyInfo in properties.Where(p => !Ignore(p)))
            {
                SetPropertyIfNotNull(poco, propertyInfo, content);
            }

            return poco;
        }

        private static void SetContentId<T>(T target, IPublishedContent content)
        {
            var propertyInfo = typeof(T).GetProperties()
             .FirstOrDefault(prop => TryGetPropertyAlias(prop, content, out string alias) && alias == "Id");

            if (propertyInfo != null)
            {
                propertyInfo.SetValue(target, content.Id);
            }
        }

        private static void SetContentName<T>(T target, IPublishedContent content)
        {
            var propertyInfo = typeof(T).GetProperties()
                .FirstOrDefault(prop => TryGetPropertyAlias(prop, content, out string alias) && alias == "Name");

            if (propertyInfo != null)
            {
                propertyInfo.SetValue(target, content.Name);
            }
        }


        private static void SetPropertyIfNotNull<T>(T target, PropertyInfo propertyInfo, IPublishedContent content)
        {
            if (TryGetPropertyAlias(propertyInfo, content, out string alias))
            {
                var contentProperty = content.GetProperty(alias);
                switch (contentProperty.PropertyType.EditorAlias)
                {
                    case "Umbraco.Label":
                    case "Umbraco.TextBox":
                    case "Umbraco.TextArea":
                    case "Umbraco.TinyMCE":
                        propertyInfo.SetValue(target, content.Value<string>(alias), null);
                        break;

                    case "Umbraco.TrueFalse":
                        propertyInfo.SetValue(target, content.Value<bool>(alias), null);
                        break;

                    case "Umbraco.DropDown.Flexible":

                        //try
                        //{
                        //    var typedValue = Convert.ChangeType(content.Value(alias), propertyInfo.PropertyType);

                        //}
                        //catch (Exception ex)
                        //{

                        //    throw new Exception($"Parameter {contentProperty.PropertyType.DataType.GetType()} cannot be converted to expected type {propertyInfo.PropertyType}");
                        //}

                        if (contentProperty.PropertyType.ClrType == typeof(IEnumerable<string>) /*If "Enable multiple choice" enabled, editors will be able to select multiple values from the dropdown otherwise only a single value can be selected.*/)
                        {

                            var multipleValues = content.Value<IEnumerable<string>>(alias);

                            propertyInfo.SetValue(target, multipleValues, null);

                        }
                        else
                        {
                            var ddlValue = content.Value<string>(alias)
                                .Replace("\"", string.Empty)
                                .Replace("[", string.Empty)
                                .Replace("]", string.Empty);

                            propertyInfo.SetValue(target, ddlValue, null);
                        }


                        break;

                    case "Umbraco.MediaPicker3":
                        //var mediaPicker = content.Value<MediaPickerUmbEditor>(alias);
                        //propertyInfo.SetValue(target, mediaPicker, null);

                        if (contentProperty.PropertyType.ClrType == typeof(IEnumerable<MediaWithCrops>) /*"Pick multiple items" Data Type setting is disabled*/)
                        {
                            var mediaUrls = new List<string>();
                            foreach (var entry in content.Value<IEnumerable<MediaWithCrops>>(alias))
                            {
                                mediaUrls.Add(entry.MediaUrl());
                            }

                            propertyInfo.SetValue(target, mediaUrls, null);

                        }
                        else
                        {
                            var typedMediaPicker = content.Value<MediaWithCrops>(alias);
                            var mediaUrl = typedMediaPicker.MediaUrl();
                            propertyInfo.SetValue(target, mediaUrl, null);
                        }

                        break;

                    case "Umbraco.Tags":
                        var tags = content.Value<IEnumerable<string>>(alias);

                        if (tags.Any())
                        {
                            if (propertyInfo.PropertyType == typeof(IEnumerable<string>)
                                || propertyInfo.PropertyType == typeof(List<string>))
                            {
                                // If the target property is expecting an IEnumerable<string>, no conversion is needed.
                                propertyInfo.SetValue(target, tags.ToList(), null);
                            }
                            else if (propertyInfo.PropertyType == typeof(string))
                            {
                                // If the target property is a string, join the IEnumerable<string> into a single string.
                                var joinedValue = string.Join(", ", tags);
                                propertyInfo.SetValue(target, joinedValue, null);
                            }
                        }

                        break;

                    case "Umbraco.CheckBoxList":
                        var checkBoxList = content.Value<IEnumerable<string>>(alias);

                        if (checkBoxList.Any())
                        {
                            if (propertyInfo.PropertyType == typeof(IEnumerable<string>)
                                || propertyInfo.PropertyType == typeof(List<string>))
                            {
                                // If the target property is expecting an IEnumerable<string>, no conversion is needed.
                                propertyInfo.SetValue(target, checkBoxList.ToList(), null);
                            }
                            else if (propertyInfo.PropertyType == typeof(string))
                            {
                                // If the target property is a string, join the IEnumerable<string> into a single string.
                                var joinedValue = string.Join(", ", checkBoxList);
                                propertyInfo.SetValue(target, joinedValue, null);
                            }
                        }

                        break;

                    case "Umbraco.EmailAddress":
                        propertyInfo.SetValue(target, content.Value<string>(alias), null);
                        break;

                    case "Umbraco.MemberGroupPicker":
                        propertyInfo.SetValue(target, content.Value<string>(alias), null);
                        break;

                    case "Umbraco.UploadField":
                        var uploadField = content.Value<string>(alias);
                        propertyInfo.SetValue(target, uploadField, null);
                        break;

                    case "Umbraco.ColorPicker":
                        propertyInfo.SetValue(target, content.Value<string>(alias), null);
                        break;

                    case "Umbraco.ContentPicker":
                        var contentPicker = content.Value<IPublishedContent>(alias);
                        if (contentPicker != null)
                        {

                        }
                        break;

                    case "Umbraco.MultiNodeTreePicker":
                        var multiNodeTreePicker = content.Value<IEnumerable<IPublishedContent>>(alias);
                        if (multiNodeTreePicker != null)
                        {

                        }
                        break;

                    case "Umbraco.MemberPicker":
                        var memberPicker = content.Value<IPublishedContent>(alias);
                        if (memberPicker != null)
                        {

                        }
                        break;


                    case "Umbraco.MultiUrlPicker":
                        if (contentProperty.PropertyType.ClrType == typeof(IEnumerable<Link>) /*This property editor returns a single item if the "Maximum number of items" Data Type setting is set to 1 or a collection if it is 0*/)
                        {
                            var links = content.Value<IEnumerable<Link>>(alias);
                            propertyInfo.SetValue(target, links, null);
                        }
                        else
                        {
                            var link = content.Value<Link>(alias);
                            propertyInfo.SetValue(target, link, null);
                        }

                        break;

                    case "Umbraco.UserPicker":
                        var userPicker = content.Value<IPublishedContent>(alias);
                        if (userPicker != null)
                        {

                        }
                        break;


                    case "Umbraco.RadioButtonList":
                        var radioButtonValue = content.Value<string>(alias);
                        propertyInfo.SetValue(target, radioButtonValue, null);
                        break;

                    case "Umbraco.Decimal":
                        var decimalValue = content.Value<decimal>(alias);
                        propertyInfo.SetValue(target, decimalValue, null);
                        break;

                    case "Umbraco.Integer":
                        var intValue = content.Value<int>(alias);
                        propertyInfo.SetValue(target, intValue, null);
                        break;

                    case "Umbraco.DateTime":
                        var dateTimeValue = content.Value<DateTime>(alias);
                        propertyInfo.SetValue(target, dateTimeValue, null);
                        break;

                    case "Umbraco.Listview":
                        var listView = content.Value<IEnumerable<IPublishedContent>>(alias);
                        if (listView != null)
                        {

                        }
                        break;

                    default:
                        var propertyValue = contentProperty.GetValue();
                        if (contentProperty.PropertyType.ClrType == propertyInfo.PropertyType)
                        {
                            propertyInfo.SetValue(target, propertyValue);
                        }
                        break;
                }
            }
        }


        private static bool TryGetPropertyAlias(PropertyInfo propertyInfo, IPublishedContent content, out string alias)
        {
            if (content.HasProperty(propertyInfo.Name))
            {
                alias = propertyInfo.Name;
                return true;
            }

            var umbAttr = propertyInfo.GetCustomAttribute<UmbAlias>();
            if (umbAttr != null)
            {
                alias = umbAttr.Alias;
                return true;
            }

            alias = string.Empty;
            return false;
        }


        private static bool Ignore(PropertyInfo propertyInfo)
        {
            var umbIgnore = propertyInfo.GetCustomAttribute<UmbIgnore>();
            return umbIgnore != null;
        }

        private static bool IsEnumerable(PropertyInfo pi)
        {
            return pi.PropertyType != typeof(string) && pi.PropertyType.IsArray;
        }


    }
}

