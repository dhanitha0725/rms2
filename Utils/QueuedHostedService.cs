using Application.Abstractions.Interfaces;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Utilities
{
    public class QueuedHostedService(
        IBackgroundTaskQueue queue,
        ILogger logger) 
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await queue.DequeueAsync(stoppingToken);
                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error occurred executing task.");
                }
            }
        }
    }
}
