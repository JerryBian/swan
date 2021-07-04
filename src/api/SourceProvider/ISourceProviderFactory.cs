using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laobian.Api.SourceProvider
{
    public interface ISourceProviderFactory
    {
        ISourceProvider Get(SourceMode source);
    }
}
