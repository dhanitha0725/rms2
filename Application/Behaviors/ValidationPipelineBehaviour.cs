using FluentValidation;
using MediatR;

namespace Application.Behaviors
{
    public class ValidationPipelineBehaviour <TRequest, TRespond> : IPipelineBehavior<TRequest, TRespond>
            where TRequest : IRequest
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationPipelineBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TRespond> Handle(TRequest request, 
            RequestHandlerDelegate<TRespond> next, 
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
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
                throw new ValidationException(validationFaliures);
            }

            //if there are no validation failures, continue with the request
            return await next();
        }
    }
}
