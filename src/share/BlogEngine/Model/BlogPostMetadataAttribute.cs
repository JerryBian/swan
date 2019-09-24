using System;

namespace Laobian.Share.BlogEngine.Model
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BlogPostMetadataAttribute : Attribute
    {
        public BlogPostMetadataAttribute(BlogPostMetadataReturnType returnType, params string[] alias)
        {
            ReturnType = returnType;
            Alias = alias ?? new string[0];
            IsAssignable = true;
        }

        public BlogPostMetadataReturnType ReturnType { get; set; }

        public string[] Alias { get; set; }

        public bool IsAssignable { get; set; }
    }
}
