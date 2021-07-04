using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Laobian.Api.SourceProvider
{
    public class SourceProviderFactory : ISourceProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SourceProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ISourceProvider Get(SourceMode source)
        {
            switch (source)
            {
                case SourceMode.Local:
                    return _serviceProvider.GetService<LocalFileSourceProvider>();
                case SourceMode.GitHub:
                    return _serviceProvider.GetService<GitHubSourceProvider>();
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
