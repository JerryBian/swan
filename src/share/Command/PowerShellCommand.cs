using System;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Command
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
            using var ps = PowerShell.Create();
            ps.AddScript(command);
            ps.AddCommand("Out-String");

            var result = await ps.InvokeAsync();
            if (result.Any())
            {
                _logger.LogInformation(
                    $"Out Stream::Executed command {command}, the output is:{Environment.NewLine}{string.Join(Environment.NewLine, result)}");
            }

            if (ps.Streams.Error.Count > 0)
            {
                _logger.LogError(
                    $"Error Stream::Executed command {command} having errors, the output is:{Environment.NewLine}{string.Join(Environment.NewLine, ps.Streams.Error)}");
            }
        }
    }
}