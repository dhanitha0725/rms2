using Domain.Common;
using MediatR;

namespace Application.Features.ManagePackages.AddRoomPricing
{
    public class AddRoomPricingCommand : IRequest<Result>
    {
        public int FacilityId { get; set; }
        public int RoomTypeId { get; set; }
        public Dictionary<string, decimal> Pricings { get; set; } = new();
    }
}
