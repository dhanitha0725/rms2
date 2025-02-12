using Application.Abstractions.Interfaces;
using Application.DTOs.FacilityDtos;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Persistence.Repositories
{
    public class FacilityRepository : IFacilityRepository
    {
        public Task<Result<List<FacilityTypeDto>>> GetFacilityTypesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
