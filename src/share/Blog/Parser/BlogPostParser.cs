using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
            var result = new BlogAssetParseResult<BlogPost>(parseResult);
            var blogPostRaw = new BlogPostRaw();
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

                        prop.SetValue(blogPostRaw, boolVal);
                        break;
                    case BlogAssetMetaReturnType.DateTime:
                        if (DateTime.TryParse(nameValue.Value, out var timeVal))
                        {
                            prop.SetValue(blogPostRaw, timeVal);
                        }

                        break;
                    case BlogAssetMetaReturnType.Int32:
                        if (int.TryParse(nameValue.Value, out var intVal))
                        {
                            prop.SetValue(blogPostRaw, intVal);
                        }

                        break;
                    case BlogAssetMetaReturnType.String:
                        prop.SetValue(blogPostRaw, nameValue.Value);
                        break;
                    case BlogAssetMetaReturnType.ListOfString:
                        var parts = nameValue.Value.Split(_appConfig.Common.PeriodSplitter);
                        prop.SetValue(blogPostRaw, parts.ToList());
                        break;
                    default:
                        result.Success = false;
                        result.ErrorMessages.Add($"Not supported return type: \"{propMeta.ReturnType}\".");
                        return result;
                }
            }

            result.Instance = new BlogPost { Raw = blogPostRaw };
            return result;
        }
    }
}
