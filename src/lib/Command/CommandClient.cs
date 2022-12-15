using Swan.Lib.Extension;
using System.Diagnostics;
using System.Text;

namespace Swan.Lib.Command
{
    public class CommandClient : ICommandClient
    {
        public async Task<string> RunAsync(string command, CancellationToken cancellationToken = default)
        {
            string scriptFile = await CreateFileAsync(command);
            using Process process = new();
            using ManualResetEventSlim outputEvent = new(true);
            using ManualResetEventSlim errorEvent = new(true);
            process.StartInfo = new ProcessStartInfo
            {
                FileName = GetShellName(),
                Arguments = $"{GetShellOption()} \"{scriptFile}\"",
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            };

            StringBuilder result = new();
            process.OutputDataReceived += (_, args) =>
            {
                if (args.Data == null && !outputEvent.WaitHandle.SafeWaitHandle.IsClosed)
                {
                    outputEvent.Set();
                }
                else
                {
                    _ = result.AppendLine(args.Data);
                }
            };
            process.ErrorDataReceived += (_, args) =>
            {
                if (args.Data == null && !errorEvent.WaitHandle.SafeWaitHandle.IsClosed)
                {
                    errorEvent.Set();
                }
                else
                {
                    _ = result.AppendLine(args.Data);
                }
            };
            _ = process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            await process.WaitForExitAsync(cancellationToken).OkForCancel();
            try
            {
                outputEvent.Wait(cancellationToken);
                errorEvent.Wait(cancellationToken);
            }
            catch (OperationCanceledException) { }

            if (File.Exists(scriptFile))
            {
                File.Delete(scriptFile);
            }

            try
            {
                process.Kill(true);
            }
            catch { }

            return result.ToString();
        }

        private async Task<string> CreateFileAsync(string script)
        {
            string tmpFile = Path.GetTempFileName();
            string scriptFile = $"{tmpFile}{GetFileExt()}";
            await File.WriteAllTextAsync(scriptFile, script, new UTF8Encoding(false));
            return scriptFile;
        }

        private string GetFileExt()
        {
            return OperatingSystem.IsWindows()
                ? ".bat"
                : OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()
                ? ".sh"
                : throw new NotSupportedException($"The platform({Environment.OSVersion}) is not supported yet.");
        }

        private string GetShellOption()
        {
            return OperatingSystem.IsWindows()
                ? "/q /c"
                : OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()
                ? ""
                : throw new NotSupportedException($"The platform({Environment.OSVersion}) is not supported yet.");
        }

        private string GetShellName()
        {
            return OperatingSystem.IsWindows()
                ? "cmd.exe"
                : OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()
                ? "/bin/bash"
                : throw new NotSupportedException($"The platform({Environment.OSVersion}) is not supported yet.");
        }
    }
}
