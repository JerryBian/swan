using System;
using System.Collections.Generic;
using System.Text;
using Laobian.Share.Config;

namespace Laobian.Share.BlogEngine.Model
{
    public abstract class BlogAsset
    {
        public AppConfig Config { get; private set; }

        public void SetConfig(AppConfig config)
        {
            Config = config;
        }
    }
}
