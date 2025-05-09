using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ManageFacility.UploadImages
{
    public record AddFacilityImagesCommand (
        int FacilityId,
        List<IFormFile> Files,
        string ContainerName = "rmscontainer") : 
        IRequest<Result<List<string>>>
    {
    }
}
