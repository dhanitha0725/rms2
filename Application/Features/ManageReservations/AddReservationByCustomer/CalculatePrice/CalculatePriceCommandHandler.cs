using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManageReservations.AddReservationByCustomer.CalculatePrice
{
    public class CalculatePriceCommandHandler (
        IGenericRepository<Pricing, int> pricingRepository,
        ILogger logger) 
        : IRequestHandler<CalculatePriceCommand, Result<decimal>>
    {
        public async Task<Result<decimal>> Handle(
            CalculatePriceCommand request, 
            CancellationToken cancellationToken)
        {
            decimal total = 0;
            var durationDays = (request.EndDate - request.StartDate).Days;

            foreach (var selectedPackage in request.Packages)
            {
                var pricing = (await pricingRepository.GetAllAsync(cancellationToken))
                    .FirstOrDefault(p => p.PackageID == selectedPackage.PackageId && p.Sector == request.CustomerType);

                if (pricing == null)
                {
                    return Result<decimal>.Failure(new Error($"Pricing for package {selectedPackage.PackageId} and sector {request.CustomerType} not found"));
                }

                total += pricing.Price * selectedPackage.Quantity * (durationDays > 0 ? durationDays: 1);
            }

            return Result<decimal>.Success(total);
        }
    }
}
