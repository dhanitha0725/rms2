using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.AddFacility.Commands
{
    public class AddFacilityCommand : IRequest<Result<int>>
    {
        public AddFacilityDto FacilityDto { get; set; }
    }
}
