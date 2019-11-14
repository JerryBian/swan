using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Laobian.Share.Blog.Parser
{
    public class BlogAssetParser
    {
        public virtual async Task<BlogAssetParseResult> FromTextAsync(
            string text,
            string nameValueSplitter,
            string lineSplitter = null)
        {
            var result = new BlogAssetParseResult();
            var hasLineSplitter = !string.IsNullOrEmpty(lineSplitter);
            using (var sr = new StringReader(text))
            {
                var line = await sr.ReadLineAsync();
                if (hasLineSplitter)
                {
                    if (!line.StartsWith(lineSplitter, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Success = false;
                        result.ErrorMessages.Add($"First line must starts with \"{lineSplitter}\".");
                        return result;
                    }

                    line = await sr.ReadLineAsync();
                }

                while (line != null)
                {
                    if (hasLineSplitter && line.StartsWith(lineSplitter, StringComparison.OrdinalIgnoreCase))
                    {
                        result.UnParsedContent = await sr.ReadToEndAsync();
                        return result;
                    }

                    var splitterIndex = line.IndexOf(nameValueSplitter, StringComparison.OrdinalIgnoreCase);
                    if (splitterIndex < 0)
                    {
                        result.WarningMessages.Add($"Skip line. No splitter found for line \"{line}\".");
                    }
                    else
                    {
                        var name = line.Substring(0, splitterIndex).Trim();
                        var value = line.Substring(splitterIndex + nameValueSplitter.Length).Trim();
                        if (result.NameValues.ContainsKey(name))
                        {
                            result.WarningMessages.Add($"Duplicate name \"{name}\", last one wins.");
                        }

                        result.NameValues[name] = value;
                    }

                    line = await sr.ReadLineAsync();
                }
            }

            return result;
        }

        public virtual async Task<string> ToTextAsync(
            Dictionary<string, string> nameValues, 
            string nameValueSplitter,
            string lineSplitter = null, 
            string unParsedContent = null)
        {
            using (var sw = new StringWriter())
            {
                var hasLineSplitter = !string.IsNullOrEmpty(lineSplitter);
                if (hasLineSplitter)
                {
                    await sw.WriteLineAsync(lineSplitter);
                }

                foreach (var nameValue in nameValues)
                {
                    await sw.WriteLineAsync($"{nameValue.Key}{nameValueSplitter} {nameValue.Value}");
                }

                if (hasLineSplitter)
                {
                    await sw.WriteLineAsync(lineSplitter);
                }

                if (!string.IsNullOrEmpty(unParsedContent))
                {
                    await sw.WriteAsync(unParsedContent);
                }

                return sw.ToString();
            }
        }
    }
}
