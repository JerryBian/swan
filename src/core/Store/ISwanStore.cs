using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swan.Core.Store
{
    public interface ISwanStore
    {
        Task AddPageHitAsync(string url, string ip);

        Task<List<string>> GetPageHitsAsync();
    }
}
