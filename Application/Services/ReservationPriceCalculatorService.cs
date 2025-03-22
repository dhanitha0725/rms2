using Application.Abstractions.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class ReservationPriceCalculatorService : IReservationPriceCalculatorService
    {
        public decimal CalculateTotal(
            List<Package> packages,
            List<ReservedPackageRequest> reservedPackages, 
            string userType, 
            DateTime startDate, 
            DateTime endDate)
        {
            decimal total = 0;
            foreach (var package in packages)
            {
                var pricing = package.Pricings
                    .FirstOrDefault(p => p.Sector == userType);

                if (pricing == null) continue;

                var quantity = reservedPackages
                    .First(rp => rp.PackageId == package.PackageID)
                    .Quantity;

                var duration = (endDate - startDate).Days;

                total += pricing.Price * quantity * duration;
            }

            return total;
        }
    }
}
