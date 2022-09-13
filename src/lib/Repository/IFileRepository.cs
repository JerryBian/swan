using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laobian.Lib.Repository
{
    public interface IFileRepository
    {
        Task AddAsync(string subFolder, string fileName, byte[] content);
    }
}
