using Domain.Entities;

namespace Application.Abstractions.Interfaces
{
    public interface IReservationPriceCalculatorService
    {
        decimal CalculateTotal(
            List<Package> packages,
            List<ReservedPackageRequest> reservedPackages,
            string userType,
            DateTime startDate,
            DateTime endDate);
    }

    public sealed record ReservedPackageRequest(
        int PackageId,
        int Quantity
    );
}
