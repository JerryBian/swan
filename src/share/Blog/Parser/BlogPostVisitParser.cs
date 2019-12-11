using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;

namespace Laobian.Share.Blog.Parser
{
    public class BlogPostVisitParser : BlogAssetParser
    {
        public async Task<BlogAssetParseResult<BlogPostAccess>> FromTextAsync(string text)
        {
            var parseResult = await FromTextAsync(text, Global.Config.Common.ColonSplitter);
            var result = new BlogAssetParseResult<BlogPostAccess>(parseResult){Instance = new BlogPostAccess()};

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

        public async Task<string> ToTextAsync(BlogPostAccess postAccess)
        {
            var nameValues = new Dictionary<string, string>();
            foreach (var item in postAccess.Dump())
            {
                nameValues.Add(item.Key, item.Value.ToString());
            }

            return await ToTextAsync(nameValues, Global.Config.Common.ColonSplitter);
        }
    }
}