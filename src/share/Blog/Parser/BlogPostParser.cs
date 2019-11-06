using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;
using Laobian.Share.Config;
using Microsoft.Extensions.Options;

namespace Laobian.Share.Blog.Parser
{
    public class BlogPostParser : BlogAssetParser
    {
        private readonly AppConfig _appConfig;
        private readonly Dictionary<BlogAssetMetaAttribute, PropertyInfo> _metaProperties;

        public BlogPostParser(IOptions<AppConfig> appConfig)
        {
            _appConfig = appConfig.Value;
            _metaProperties = new Dictionary<BlogAssetMetaAttribute, PropertyInfo>();
            foreach (var propertyInfo in typeof(BlogPostRaw).GetProperties())
            {
                var attribute = propertyInfo.GetCustomAttribute<BlogAssetMetaAttribute>();
                if (attribute != null)
                {
                    _metaProperties.Add(attribute, propertyInfo);
                }
            }
        }

        public async Task<BlogAssetParseResult<BlogPost>> FromTextAsync(string text)
        {
            var parseResult =
                await FromTextAsync(text, _appConfig.Common.ColonSplitter, _appConfig.Blog.MetadataSplitter);
            var result = new BlogAssetParseResult<BlogPost>(parseResult) { Instance = new BlogPost() };

            foreach (var nameValue in result.NameValues)
            {
                var propMeta = _metaProperties.Keys.FirstOrDefault(k =>
                    k.Alias.Contains(nameValue.Key, StringComparer.OrdinalIgnoreCase));
                if (propMeta == null)
                {
                    continue;
                }

                var prop = _metaProperties[propMeta];
                switch (propMeta.ReturnType)
                {
                    case BlogAssetMetaReturnType.Bool:
                        if (!bool.TryParse(nameValue.Value, out var boolVal))
                        {
                            boolVal = false;
                        }

                        prop.SetValue(result.Instance.Raw, boolVal);
                        break;
                    case BlogAssetMetaReturnType.DateTime:
                        if (DateTime.TryParse(nameValue.Value, out var timeVal))
                        {
                            prop.SetValue(result.Instance.Raw, timeVal);
                        }

                        break;
                    case BlogAssetMetaReturnType.Int32:
                        if (int.TryParse(nameValue.Value, out var intVal))
                        {
                            prop.SetValue(result.Instance.Raw, intVal);
                        }

                        break;
                    case BlogAssetMetaReturnType.String:
                        prop.SetValue(result.Instance.Raw, nameValue.Value);
                        break;
                    case BlogAssetMetaReturnType.ListOfString:
                        var parts = nameValue.Value.Split(_appConfig.Common.PeriodSplitter);
                        prop.SetValue(result.Instance.Raw, parts.ToList());
                        break;
                    default:
                        result.Success = false;
                        result.ErrorMessages.Add($"Not supported return type: \"{propMeta.ReturnType}\".");
                        return result;
                }
            }

            return result;
        }

        public async Task<string> ToTextAsync(BlogPost post)
        {
            var nameValues = new Dictionary<string, string>();
            foreach (var item in _metaProperties)
            {
                string value;
                switch (item.Key.ReturnType)
                {
                    case BlogAssetMetaReturnType.Bool:
                    case BlogAssetMetaReturnType.Int32:
                    case BlogAssetMetaReturnType.String:
                        value = Convert.ToString(item.Value.GetValue(post.Raw));
                        break;
                    case BlogAssetMetaReturnType.DateTime:
                        value = Convert.ToDateTime(item.Value.GetValue(post.Raw)).ToString("yyyy-MM-dd hh:mm:ss");
                        break;
                    case BlogAssetMetaReturnType.ListOfString:
                        value = string.Join(_appConfig.Common.PeriodSplitter, (List<string>)item.Value.GetValue(post.Raw));
                        break;
                    default:
                        value = string.Empty;
                        break;
                }

                if (!string.IsNullOrEmpty(value))
                {
                    nameValues[item.Key.Alias.First()] = value;
                }
            }

            return await ToTextAsync(nameValues, _appConfig.Common.ColonSplitter, _appConfig.Blog.MetadataSplitter,
                post.ContentMarkdown);
        }

    }
}
