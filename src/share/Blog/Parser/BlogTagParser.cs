using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;

namespace Laobian.Share.Blog.Parser
{
    public class BlogTagParser : BlogAssetParser
    {
        public async Task<BlogAssetParseResult<List<BlogTag>>> FromTextAsync(string text)
        {
            var parseResult = await base.FromTextAsync(text, Global.Config.Common.ColonSplitter);
            var result = new BlogAssetParseResult<List<BlogTag>>(parseResult) {Instance = new List<BlogTag>()};

            if (!result.Success)
            {
                return result;
            }

            foreach (var nameValue in result.NameValues)
            {
                result.Instance.Add(new BlogTag
                {
                    Name = nameValue.Key,
                    Link = nameValue.Value
                });
            }

            return result;
        }
    }
}