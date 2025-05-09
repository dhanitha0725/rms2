using FluentValidation;
using MediatR;
using Serilog;

namespace Application.Behaviors
{
    public class ValidationPipelineBehaviour <TRequest, TRespond> : 
            IPipelineBehavior<TRequest, TRespond>
            where TRequest : IRequest<TRespond>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger _logger;

        public ValidationPipelineBehaviour(
            IEnumerable<IValidator<TRequest>> validators,
            ILogger logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public async Task<TRespond> Handle(
            TRequest request, 
            RequestHandlerDelegate<TRespond> next, 
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                _logger.Information("No Validations");
                return await next();
            }

            //create a validation context for the request
            var context = new ValidationContext<TRequest>(request);

            //run all validators asynchronously
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var validationFaliures = validationResults
                .Where(result => result.Errors.Any())
                .SelectMany(result => result.Errors)
                .ToList();

            //if there are validation failures, throw an exception
            if (validationFaliures.Any())
            {
                _logger.Error("Validation Failure.");
                throw new ValidationException(validationFaliures);
            }

            //if there are no validation failures, continue with the request
            return await next();
        }
    }
}
