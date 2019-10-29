using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.BlogEngine.Model;
using Laobian.Share.Config;

namespace Laobian.Share.BlogEngine.Parser
{
    public class BlogTagParser : BlogAssetParser
    {
        public BlogTagParser(AppConfig appConfig) : base(appConfig)
        {
        }

        public async Task<List<BlogTag>> FromTextAsync(string text)
        {
            var result = new List<BlogTag>();
            var parseResult = await base.FromTextAsync(text, Config.Common.ColonSplitter);
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
