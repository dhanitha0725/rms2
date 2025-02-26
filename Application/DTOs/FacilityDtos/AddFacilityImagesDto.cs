using Microsoft.AspNetCore.Http;

namespace Application.DTOs.FacilityDtos
{
    public class AddFacilityImagesDto
    {
        public List<IFormFile> Images { get; set; }
    }
}
