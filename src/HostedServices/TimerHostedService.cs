using ByteSizeLib;
using Swan.Core.Extension;
using System.Diagnostics;

namespace Swan.HostedServices
{
    public class TimerHostedService : BackgroundService
    {
        private DateTime _startAt;
        private readonly ILogger<TimerHostedService> _logger;

        public TimerHostedService(ILogger<TimerHostedService> logger)
        {
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _startAt = DateTime.Now;
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken).OkForCancel();

                using var p = Process.GetCurrentProcess();
                _logger.LogInformation($"=== App metrics === " +
                    $"Started at: {_startAt.ToFullDateTime()}, " +
                    $"Running for: {DateTime.Now - _startAt}, " +
                    $"Processor count:{Environment.ProcessorCount}, " +
                    $"Is 64 bit OS:{Environment.Is64BitOperatingSystem}, " +
                    $"Is 64 bit process: {Environment.Is64BitProcess}, " +
                    $"OS version: {Environment.OSVersion}, " +
                    $"System page size: {ByteSize.FromBytes(Environment.SystemPageSize)}, " +
                    $"System running for: {TimeSpan.FromMilliseconds(Environment.TickCount64)}, " +
                    $"CLR version: {Environment.Version}, " +
                    $"Threads count: {p.Threads.Count}, " +
                    $"Non paged system memory: {ByteSize.FromBytes(p.NonpagedSystemMemorySize64)}, " +
                    $"Paged memory: {ByteSize.FromBytes(p.PagedMemorySize64)}, " +
                    $"Pageable system memory: {ByteSize.FromBytes(p.PagedSystemMemorySize64)}, " +
                    $"Maximum used virtual memory in paging file: {ByteSize.FromBytes(p.PeakPagedMemorySize64)}, " +
                    $"Maximum used virtual memory: {ByteSize.FromBytes(p.PeakVirtualMemorySize64)}, " +
                    $"Maximum used physical memory: {ByteSize.FromBytes(p.PeakWorkingSet64)}, " +
                    $"Allocated private memory: {ByteSize.FromBytes(p.PrivateMemorySize64)}, " +
                    $"Allocated virtual memory: {ByteSize.FromBytes(p.VirtualMemorySize64)}, " +
                    $"Allocated physical memory: {ByteSize.FromBytes(p.WorkingSet64)}, " +
                    $"Total processor time: {p.TotalProcessorTime}, " +
                    $"User processor time: {p.UserProcessorTime}, " +
                    $"Privileged processor time: {p.PrivilegedProcessorTime}.");
            }
        }
    }
}
