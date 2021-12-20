using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Logger;
using Laobian.Share.Site;

namespace Laobian.Api.Service;

public interface ILogFileService
{
    Task<List<LaobianLog>> GetLogsAsync(LaobianSite site, DateTime date,
        CancellationToken cancellationToken = default);

    Task AddLogAsync(LaobianLog log, CancellationToken cancellationToken = default);
}