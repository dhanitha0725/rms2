using Application.Abstractions.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class PricingService : IPricingService
    {
        public async Task<decimal> CalculatePriceAsync(List<Package> packages, List<Room> rooms, string userType)
        {
            decimal total = 0;

            foreach (var package in packages)
            {
            }

            return total;
        }
    }
}
