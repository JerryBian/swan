using System;
using System.Collections.Generic;
using System.Text;

namespace Laobian.Share.Blog.Model
{
    public class InvalidBlogAssetException : Exception
    {
        public InvalidBlogAssetException(string propertyName) : base($"Invalid property {propertyName}.")
        {
        }
    }
}
