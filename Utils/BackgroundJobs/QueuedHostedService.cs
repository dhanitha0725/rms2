using Application.Abstractions.Interfaces;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Utilities.BackgroundJobs
{
    public class QueuedHostedService(
        IBackgroundTaskQueue queue,
        ILogger logger) 
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.Information("Background task hosted service starting.");
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
            logger.Information("Background task hosted service stopping.");
        }
    }
}
