using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laobian.Lib.Service
{
    public interface IFileService
    {
        Task<string> AddAsync(string fileName, byte[] content);
    }
}
