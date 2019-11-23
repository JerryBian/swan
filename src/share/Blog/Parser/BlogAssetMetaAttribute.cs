using System;

namespace Laobian.Share.Blog.Parser
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BlogAssetMetaAttribute : Attribute
    {
        public BlogAssetMetaAttribute(BlogAssetMetaReturnType returnType, params string[] alias)
        {
            ReturnType = returnType;
            Alias = alias;
        }

        public BlogAssetMetaReturnType ReturnType { get; set; }

        public string[] Alias { get; set; }
    }
}