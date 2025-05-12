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

            // Schedule the task to run every 5 minutes
            _timer = new Timer(
                async (state) => await EnqueueReservationCompletionTask(),
                null,
                TimeSpan.Zero,           // Start immediately
                TimeSpan.FromMinutes(5)  // Repeat every 5 minutes
            );

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Stopping reservation completion scheduler.");
            _timer?.Change(Timeout.Infinite, 0); // Stop the timer
            _timer?.Dispose();                   // Dispose of the timer
            return Task.CompletedTask;
        }

        private async Task EnqueueReservationCompletionTask()
        {
            _queue.QueueBackgroundWorkItem(async (cancellationToken) =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var task = scope.ServiceProvider.GetRequiredService<ReservationCompletionTask>();
                await task.Execute(cancellationToken);
            });
            _logger.Information("reservation completion task done.");
        }
    }
}