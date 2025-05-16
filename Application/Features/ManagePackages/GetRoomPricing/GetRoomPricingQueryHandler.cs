using Application.DTOs.PackageDto;
using Domain.Common;
using MediatR;

namespace Application.Features.ManagePackages.GetRoomPricing
{
    public class GetRoomPricingQueryHandler : IRequestHandler<GetRoomPricingQuery, Result<RoomPricingTableDto>>
    {
        public async Task<Result<RoomPricingTableDto>> Handle(
            GetRoomPricingQuery request, 
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
