using Domain.Common;
using MediatR;
using Serilog;

namespace Application.Behaviors
{
    public class LoggingPipelineBehavior<TRequest, TResponse> (ILogger logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : Result
    {
        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            logger.Information(
                "Starting Request: {@Result}, {@DateTimeUtc}",
                typeof(TRequest).Name, DateTime.UtcNow);

            var result = await next();

            if (!result.IsSuccess)
            {
                logger.Error(
                    "Failed Request: {@Result}, {@DateTimeUtc}",
                    typeof(TRequest).Name, DateTime.UtcNow);
            }

            logger.Information(
                "Completed Request: {@Result}, {@DateTimeUtc}",
                typeof(TRequest).Name, DateTime.UtcNow);

            return result;
        }
    }
}
