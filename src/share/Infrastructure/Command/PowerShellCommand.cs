using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Laobian.Share.Infrastructure.Command
{
    public class PowerShellCommand : ICommand
    {
        public async Task<List<string>> ExecuteAsync(string command)
        {
            using (var ps = PowerShell.Create())
            {
                var results = await ps.AddScript(command).InvokeAsync();
                return results.Select(r => r.ToString()).ToList();
            }
        }
    }
}
