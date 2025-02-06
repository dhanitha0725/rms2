using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.AddFacility.Commands
{
    public class AddFacilityCommand : IRequest<Result<FacilityResponseDto>>
    {
        public AddFacilityDto Facility { get; set; }

        public AddFacilityCommand(AddFacilityDto facility)
        {
            Facility = facility;
        }
    }
}
