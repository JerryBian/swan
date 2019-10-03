using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Laobian.Share.BlogEngine.Model;
using Laobian.Share.Config;

namespace Laobian.Share.BlogEngine.Parser
{
    public class BlogPostParser : BlogAssetParser
    {
        private readonly AppConfig _appConfig;
        private readonly Dictionary<string, Tuple<BlogPostMetadataAttribute, PropertyInfo>> _props;

        public BlogPostParser(AppConfig appConfig)
        {
            _appConfig = appConfig;
            _props = new Dictionary<string, Tuple<BlogPostMetadataAttribute, PropertyInfo>>(StringComparer.OrdinalIgnoreCase);
            foreach (var propertyInfo in typeof(BlogPost).GetProperties())
            {
                var attr = propertyInfo.GetCustomAttributes<BlogPostMetadataAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    foreach (var al in attr.Alias)
                    {
                        _props.Add(al, new Tuple<BlogPostMetadataAttribute, PropertyInfo>(attr, propertyInfo));
                    }
                }
            }
        }

        public async Task<BlogPost> FromTextAsync(string text, string link)
        {
            var parseResult = await FromTextAsync(text, BlogConstant.ColonSplitter, BlogConstant.MetadataLine);
            var blogPost = new BlogPost();
            foreach (var item in parseResult.Item1)
            {
                if (_props.ContainsKey(item.Key))
                {
                    var propValue = _props[item.Key];
                    var value = item.Value;
                    switch (propValue.Item1.ReturnType)
                    {
                        case BlogPostMetadataReturnType.DateTime:
                            if (DateTime.TryParse(value, out var result))
                            {
                                propValue.Item2.SetValue(blogPost, result);
                            }
                            break;
                        case BlogPostMetadataReturnType.ListOfString:
                            var rs = value.Split(BlogConstant.PeriodSplitter).Select(_ => _.Trim()).ToList();
                            propValue.Item2.SetValue(blogPost, rs);
                            break;
                        case BlogPostMetadataReturnType.String:
                            propValue.Item2.SetValue(blogPost, value.Trim());
                            break;
                        case BlogPostMetadataReturnType.Bool:
                            if (!bool.TryParse(value.Trim(), out var boolVal))
                            {
                                boolVal = false;
                            }

                            propValue.Item2.SetValue(blogPost, boolVal);
                            break;
                        case BlogPostMetadataReturnType.Int32:
                            propValue.Item2.SetValue(blogPost, int.TryParse(value, out var intVal) ? intVal : default);
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }
            }

            blogPost.Link = link;
            blogPost.MarkdownContent = parseResult.Item2;
            blogPost.LocalFullPath = Path.Combine(_appConfig.AssetRepoLocalDir, blogPost.GitHubPath);
            blogPost.SetDefaults();
            return blogPost;
        }

        public async Task<string> ToTextAsync(BlogPost post)
        {
            var nameValues = new List<KeyValuePair<string, string>>();
            foreach (var item in _props.Values)
            {
                var attr = item.Item1;
                var propertyInfo = item.Item2;
                if (attr != null)
                {
                    if (nameValues.Any(
                        _ => string.Equals(_.Key, attr.Alias.First(), StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    string value;
                    switch (attr.ReturnType)
                    {
                        case BlogPostMetadataReturnType.DateTime:
                            value = ((DateTime)propertyInfo.GetValue(post)).ToString("yyyy-MM-dd hh:mm:ss");
                            break;
                        case BlogPostMetadataReturnType.ListOfString:
                            value = string.Join(",", (List<string>)propertyInfo.GetValue(post));
                            break;
                        case BlogPostMetadataReturnType.String:
                        case BlogPostMetadataReturnType.Bool:
                        case BlogPostMetadataReturnType.Int32:
                            value = propertyInfo.GetValue(post).ToString();
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                    nameValues.Add(new KeyValuePair<string, string>(attr.Alias.First(), value));
                }
            }

            return await ToTextAsync(nameValues, BlogConstant.ColonSplitter, BlogConstant.MetadataLine,
                post.MarkdownContent);
        }
    }
}
