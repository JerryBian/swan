using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Command.Laobian.Share.Command;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Share.Command
{
    public class ProcessCommandClient : ICommandClient
    {
        private readonly CommonConfig _commonSetting;
        private readonly ILogger<ProcessCommandClient> _logger;

        public ProcessCommandClient(IOptions<CommonConfig> options, ILogger<ProcessCommandClient> logger)
        {
            _logger = logger;
            _commonSetting = options.Value;
        }

        public async Task<string> RunAsync(string command, CancellationToken cancellationToken = default)
        {
            using var process = new Process();
            using var resetEvent1 = new ManualResetEventSlim(false);
            using var resetEvent2 = new ManualResetEventSlim(false);
            var startInfo = new ProcessStartInfo
            {
                FileName = _commonSetting.CommandLineApp,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                Arguments = string.IsNullOrEmpty(_commonSetting.CommandLineBeginArg) ? FormatCommand(command) : $"{_commonSetting.CommandLineBeginArg} {FormatCommand(command)}"
            };

            _logger.LogInformation($"FileName={startInfo.FileName}, Args={startInfo.Arguments}");
            process.StartInfo = startInfo;

            var output = new StringBuilder();
            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data == null)
                {
                    // ReSharper disable once AccessToDisposedClosure
                    resetEvent1.Set();
                }
                else
                    output.AppendLine(args.Data);
            };
            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data == null)
                    // ReSharper disable once AccessToDisposedClosure
                    resetEvent2.Set();
                else
                    output.AppendLine(args.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync(cancellationToken);
            resetEvent1.Wait(cancellationToken);
            resetEvent2.Wait(cancellationToken);
            return output.ToString();
        }

        private string FormatCommand(string cmd)
        {
            return $"\"{cmd.Replace("\"", "\\\"")}\"";
        }
    }
}
