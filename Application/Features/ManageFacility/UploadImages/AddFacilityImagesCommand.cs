using Application.DTOs.FacilityDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageFacility.UploadImages
{
    public record AddFacilityImagesCommand (
        int FacilityId,
        AddFacilityImagesDto AddFacilityImagesDto): IRequest<Result<List<string>>>
    {
    }
}
