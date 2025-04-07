using Application.Abstractions.Interfaces;
using Application.DTOs.FacilityDtos;
using Domain.Common;
using Domain.Entities;
using MediatR;
using System.Text.Json;

namespace Application.Features.ManageFacility.SelectedFacilityDetails
{
    public class GetSelectedFacilityDetailsQueryHandler(
        IGenericRepository<Image, int> imageRepository,
        IGenericRepository<Facility, int> facilityRepository)
        : IRequestHandler<GetSelectedFacilityDetailsQuery, Result<SelectedFacilityDetailsDto>>
    {
        public async Task<Result<SelectedFacilityDetailsDto>> Handle(
            GetSelectedFacilityDetailsQuery request,
            CancellationToken cancellationToken)
        {
            // get facility by id
            var facility = await facilityRepository.GetByIdAsync(request.FacilityId, cancellationToken);

            if (facility == null)
            {
                return Result<SelectedFacilityDetailsDto>.Failure(new Error("Facility not found"));
            }

            // get images
            var images = await imageRepository.GetAllAsync(cancellationToken);
            var imageUrls = images
                .Where(i => i.FacilityID == request.FacilityId)
                .Select(i => i.ImageUrl)
                .ToList();

            // get facility attributes
            var facilityAttributes = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(facility.Attributes))
            {
                facilityAttributes = JsonSerializer.Deserialize<Dictionary<string, string>>(facility.Attributes);
            }

            return Result<SelectedFacilityDetailsDto>.Success(new SelectedFacilityDetailsDto
            {
                FacilityId = facility.FacilityID,
                FacilityName = facility.FacilityName,
                Location = facility.Location,
                Description = facility.Description,
                Attributes = facilityAttributes,
                ImageUrls = imageUrls
            });
        }
    }
}
