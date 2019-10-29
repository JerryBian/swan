using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.BlogEngine.Model;
using Laobian.Share.Config;

namespace Laobian.Share.BlogEngine.Parser
{
    public class BlogCategoryParser : BlogAssetParser
    {
        public BlogCategoryParser(AppConfig appConfig) : base(appConfig)
        {
        }

        public async Task<List<BlogCategory>> FromTextAsync(string text)
        {
            var result = new List<BlogCategory>();
            var parseResult = await base.FromTextAsync(text, Config.Common.ColonSplitter);
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
