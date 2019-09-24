using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.BlogEngine.Model;

namespace Laobian.Share.BlogEngine.Parser
{
    public class BlogTagParser : BlogAssetParser
    {
        public async Task<List<BlogTag>> FromTextAsync(string text)
        {
            var result = new List<BlogTag>();
            var parseResult = await base.FromTextAsync(text, BlogConstant.ColonSplitter);
            foreach (var item in parseResult.Item1)
            {
                result.Add(new BlogTag
                {
                    Name = item.Key,
                    Link = item.Value
                });
            }

            return result;

        }
    }
}
