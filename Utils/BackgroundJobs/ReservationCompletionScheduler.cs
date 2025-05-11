using Application.Abstractions.Interfaces;
using Application.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Utilities.BackgroundJobs
{
    public class ReservationCompletionScheduler : IHostedService
    {
        private readonly IBackgroundTaskQueue _queue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger _logger;
        private Timer _timer; // Private field to hold the Timer instance

        // Constructor without Timer parameter
        public ReservationCompletionScheduler(
            IBackgroundTaskQueue queue,
            IServiceScopeFactory serviceScopeFactory,
            ILogger logger)
        {
            _queue = queue;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Starting reservation completion scheduler");

            _timer = new Timer(
                async (state) => await EnqueueReservationCompletionTask(),
                null,
                ScheduleTime(),           // Initial delay
                TimeSpan.FromDays(1));    // Repeat every day

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Stopping reservation completion scheduler.");
            _timer?.Change(Timeout.Infinite, 0); // Stop the timer
            _timer?.Dispose();                   // Dispose of the timer
            return Task.CompletedTask;
        }

        private static TimeSpan ScheduleTime()
        {
            var now = DateTime.UtcNow;
            var nextScheduledTime = now.Date.AddDays(1).AddMinutes(5); // Schedule for 5 minutes past midnight of the next day

            if (nextScheduledTime <= now)
            {
                nextScheduledTime = nextScheduledTime.AddDays(1); // Ensure the time is in the future
            }

            return nextScheduledTime - now;
        }

        private async Task EnqueueReservationCompletionTask()
        {
            _queue.QueueBackgroundWorkItem(async (cancellationToken) =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var task = scope.ServiceProvider.GetRequiredService<ReservationCompletionTask>();
                await task.Execute(cancellationToken);
            });
            _logger.Information("Enqueued reservation completion task.");
        }
    }
}