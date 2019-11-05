using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;
using Laobian.Share.Config;
using Microsoft.Extensions.Options;

namespace Laobian.Share.Blog.Parser
{
    public class BlogCategoryParser : BlogAssetParser
    {
        private readonly AppConfig _appConfig;

        public BlogCategoryParser(IOptions<AppConfig> appConfig)
        {
            _appConfig = appConfig.Value;
        }

        public async Task<BlogAssetParseResult<List<BlogCategory>>> FromTextAsync(string text)
        {
            var parseResult = await base.FromTextAsync(text, _appConfig.Common.ColonSplitter);
            var result = new BlogAssetParseResult<List<BlogCategory>>(parseResult) { Instance = new List<BlogCategory>() };

            if (!result.Success)
            {
                return result;
            }

            foreach (var nameValue in result.NameValues)
            {
                result.Instance.Add(new BlogCategory
                {
                    Name = nameValue.Key,
                    Link = nameValue.Value
                });
            }

            return result;
        }
    }
}
