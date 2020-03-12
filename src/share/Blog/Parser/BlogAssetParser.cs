using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Laobian.Share.Helper;

namespace Laobian.Share.Blog.Parser
{
    public static class BlogAssetParser
    {
        public static BlogAssetParseResult<T> ParseJson<T>(string json)
        {
            var result = new BlogAssetParseResult<T>();
            try
            {
                result.Instance = SerializeHelper.FromJson<T>(json);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessages.Add(ex.ToString());
            }

            return result;
        }

        public static BlogAssetParseResult<string> ToText(string text)
        {
            var result = new BlogAssetParseResult<string> {Instance = text};
            return result;
        }

        public static BlogAssetParseResult<string> ToJson<T>(T obj)
        {
            var result = new BlogAssetParseResult<string>();
            try
            {
                result.Instance = SerializeHelper.ToJson(obj, true);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessages.Add(ex.ToString());
            }

            return result;
        }

        public static async Task<BlogAssetParseResult<IDictionary<string, string>>>
            ParseColonSeparatedTextAsync(string text)
        {
            const string splitter = ":";
            var result = new BlogAssetParseResult<IDictionary<string, string>>
            {
                Instance = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            };

            try
            {
                using (var sr = new StringReader(text))
                {
                    var line = await sr.ReadLineAsync();
                    while (line != null)
                    {
                        var splitterIndex = line.IndexOf(splitter, StringComparison.OrdinalIgnoreCase);
                        if (splitterIndex < 0)
                        {
                            result.WarningMessages.Add($"Skip line. No splitter found for line \"{line}\".");
                        }
                        else
                        {
                            var name = line.Substring(0, splitterIndex).Trim();
                            var value = line.Substring(splitterIndex + splitter.Length).Trim();
                            if (result.Instance.ContainsKey(name))
                            {
                                result.WarningMessages.Add($"Duplicate name \"{name}\", last one wins.");
                            }

                            result.Instance[name] = value;
                        }

                        line = await sr.ReadLineAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessages.Add(ex.ToString());
            }

            return result;
        }
    }
}