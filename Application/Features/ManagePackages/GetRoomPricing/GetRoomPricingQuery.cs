using Application.DTOs.PackageDto;
using Domain.Common;
using MediatR;

namespace Application.Features.ManagePackages.GetRoomPricing
{
    public class GetRoomPricingQuery: IRequest<Result<RoomPricingTableDto>>
    {
    }
}
