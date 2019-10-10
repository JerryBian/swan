using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Infrastructure.Command
{
    public class PowerShellCommand : ICommand
    {
        private readonly ILogger<PowerShellCommand> _logger;

        public PowerShellCommand(ILogger<PowerShellCommand> logger)
        {
            _logger = logger;
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
                    _logger.LogInformation("Executed command {Command}, the output is {Output}", command, string.Join(Environment.NewLine, result));
                }
            }
        }
    }
}
