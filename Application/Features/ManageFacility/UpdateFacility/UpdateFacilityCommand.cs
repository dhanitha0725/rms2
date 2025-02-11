using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageFacility.UpdateFacility
{
    public class UpdateFacilityCommand : IRequest<Result<string>>
    {
        public int FacilityId { get; set; }
        public UpdateFacilityDto UpdateFacilityDto { get; set; }
    }
}
