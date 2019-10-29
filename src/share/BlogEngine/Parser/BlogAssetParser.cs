using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Share.Config;

namespace Laobian.Share.BlogEngine.Parser
{
    public abstract class BlogAssetParser
    {
        protected BlogAssetParser(AppConfig appConfig)
        {
            Config = appConfig;
        }

        public AppConfig Config { get; }

        protected virtual async Task<Tuple<List<KeyValuePair<string, string>>, string>> FromTextAsync(
            string requestText, 
            string nameValueSplitter, 
            string lineSplitter = null)
        {
            var nameValues = new List<KeyValuePair<string, string>>();
            var unParsedContent = string.Empty;
            var hasLineSplitter = !string.IsNullOrEmpty(lineSplitter);
            using (var sr = new StringReader(requestText))
            {
                string line = await sr.ReadLineAsync();
                if (hasLineSplitter)
                {
                    if (!line.StartsWith(lineSplitter, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new AssetParseException();
                    }

                    line = await sr.ReadLineAsync();
                }

                while (!string.IsNullOrEmpty(line))
                {
                    if (hasLineSplitter && line.StartsWith(lineSplitter, StringComparison.OrdinalIgnoreCase))
                    {
                        // reach to the end
                        unParsedContent = await sr.ReadToEndAsync();
                        break;
                    }

                    var splitterIndex = line.IndexOf(nameValueSplitter, StringComparison.OrdinalIgnoreCase);
                    if (splitterIndex < 0)
                    {
                        throw new AssetParseException();
                    }

                    var name = line.Substring(0, splitterIndex).Trim();
                    var value = line.Substring(splitterIndex + nameValueSplitter.Length).Trim();
                    nameValues.Add(new KeyValuePair<string, string>(name, value));

                    line = await sr.ReadLineAsync();
                }
            }

            return new Tuple<List<KeyValuePair<string, string>>, string>(nameValues, unParsedContent);
        }

        protected virtual async Task<string> ToTextAsync(
            List<KeyValuePair<string, string>> nameValues, 
            string nameValueSplitter,
            string lineSplitter = null, 
            string appendedContent = null)
        {
            await using var sw = new StringWriter();
            var hasLineSplitter = !string.IsNullOrEmpty(lineSplitter);
            if (hasLineSplitter)
            {
                await sw.WriteLineAsync(lineSplitter);
            }

            foreach (var item in nameValues.OrderBy(_ => _.Key))
            {
                await sw.WriteLineAsync($"{item.Key}{nameValueSplitter} {item.Value}");
            }

            if (hasLineSplitter)
            {
                await sw.WriteLineAsync(lineSplitter);
            }

            if (!string.IsNullOrEmpty(appendedContent))
            {
                await sw.WriteAsync(appendedContent);
            }

            return sw.ToString();
        }
    }
}
