using Domain.Common;
using MediatR;

namespace Application.Features.ManageFacility.DeleteFacility
{
    public class DeleteFacilityCommand : IRequest<Result<int>>
    {
        public int FacilityId { get; set; }
    }
}
