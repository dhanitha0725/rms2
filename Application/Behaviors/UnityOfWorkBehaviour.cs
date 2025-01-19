using Application.Abstractions.Interfaces;
using MediatR;
using Serilog;
using System.Transactions;
using ICommand = Application.Abstractions.Interfaces.ICommand;

namespace Application.Behaviors
{
    public sealed class UnityOfWorkBehaviour<TRequest, TResponse>
            : IPipelineBehavior<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public UnityOfWorkBehaviour(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            if (request is not ICommand)
            {
                return await next();
            }

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                _logger.Information("Transaction started for request {RequestName}", typeof(TRequest).Name);
                try
                {
                    var response = await next();
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transactionScope.Complete();
                    _logger.Information("Transaction completed successfully for request {RequestName}", typeof(TRequest).Name);
                    return response;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Transaction failed for request {RequestName}", typeof(TRequest).Name);
                    throw;
                }
            }
        }
    }
}
