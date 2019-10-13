using System.Collections.Generic;

namespace Laobian.Share.Infrastructure.Cache
{
    public class CacheKeyBuilder
    {
        public static string Build(SiteComponent component, string feature, params object[] parts)
        {
            var vars = new List<string> { component.ToString(), feature };

            foreach (var part in parts)
            {
                vars.Add(part.ToString());
            }

            return string.Join(".", vars);
        }
    }
}
