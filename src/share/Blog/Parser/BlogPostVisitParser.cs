using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;

namespace Laobian.Share.Blog.Parser
{
    public class BlogPostVisitParser : BlogAssetParser
    {
        public async Task<BlogAssetParseResult<BlogPostVisit>> FromTextAsync(string text)
        {
            var parseResult = await FromTextAsync(text, Global.Config.Common.ColonSplitter);
            var result = new BlogAssetParseResult<BlogPostVisit>(parseResult){Instance = new BlogPostVisit()};

            if (!parseResult.Success)
            {
                return result;
            }

            foreach (var item in parseResult.NameValues)
            {
                result.Instance.Add(item.Key, Convert.ToInt32(item.Value));
            }

            return result;
        }

        public async Task<string> ToTextAsync(BlogPostVisit postVisit)
        {
            var nameValues = new Dictionary<string, string>();
            foreach (var item in postVisit.Dump())
            {
                nameValues.Add(item.Key, item.Value.ToString());
            }

            return await ToTextAsync(nameValues, Global.Config.Common.ColonSplitter);
        }
    }
}