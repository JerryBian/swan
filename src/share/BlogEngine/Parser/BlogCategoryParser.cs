using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.BlogEngine.Model;

namespace Laobian.Share.BlogEngine.Parser
{
    public class BlogCategoryParser : BlogAssetParser
    {
        public async Task<List<BlogCategory>> FromTextAsync(string text)
        {
            var result = new List<BlogCategory>();
            var parseResult = await base.FromTextAsync(text, BlogConstant.ColonSplitter);
            foreach (var item in parseResult.Item1)
            {
                result.Add(new BlogCategory
                {
                    Name = item.Key,
                    Link = item.Value
                });
            }

            return result;

        }
    }
}
