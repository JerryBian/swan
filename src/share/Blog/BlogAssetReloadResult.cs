using System;
using System.Collections.Generic;
using System.Text;

namespace Laobian.Share.Blog
{
    public class BlogAssetReloadResult<T>
    {
        public BlogAssetReloadResult()
        {
            Success = true;
        }

        public T Result { get; set; }

        public bool Success { get; set; }

        public string Error { get; set; }

        public string Warning { get; set; }
    }
}
