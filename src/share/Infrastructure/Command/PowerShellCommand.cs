using System;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Laobian.Share.Log;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Infrastructure.Command
{
    public class PowerShellCommand : ICommand
    {
        private readonly ILogService _logService;

        public PowerShellCommand(ILogService logService)
        {
            _logService = logService;
        }

        public async Task ExecuteAsync(string command)
        {
            using (var ps = PowerShell.Create())
            {
                ps.AddScript(command);
                ps.AddCommand("Out-String");

                var result = await ps.InvokeAsync();
                if (result.Any())
                {
                    await _logService.LogInformation($"Executed command {command}, the output is:{Environment.NewLine}{string.Join(Environment.NewLine, result)}");
                }
            }
        }
    }
}
