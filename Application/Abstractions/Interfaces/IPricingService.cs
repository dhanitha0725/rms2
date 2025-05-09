using Domain.Entities;

namespace Application.Abstractions.Interfaces
{
    public interface IPricingService
    {
        Task<decimal> CalculatePriceAsync(List<Package> packages, List<Room> rooms, string userType);
    }
}
