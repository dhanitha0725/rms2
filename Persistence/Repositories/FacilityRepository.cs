using Application.Abstractions.Interfaces;
using Application.DTOs.FacilityDtos;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Persistence.Repositories
{
    public class FacilityRepository : IFacilityRepository
    {
        private readonly ReservationDbContext context;

        public FacilityRepository(ReservationDbContext context)
        {
            this.context = context;
        }

        public async Task<Result<List<FacilityTypeDto>>> GetFacilityTypesAsync()
        {
            var facilityTypes = await context.Facilities
                .Select(f => new FacilityTypeDto
                {
                    FacilityType = f.FacilityType
                })
                .Distinct()
                .ToListAsync();

            return facilityTypes == null || !facilityTypes.Any() ?
                Result<List<FacilityTypeDto>>.Failure(new Error("No facility type found."))
                : Result<List<FacilityTypeDto>>.Success(facilityTypes);
        }
    }
}
