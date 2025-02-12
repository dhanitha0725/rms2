using Application.DTOs.FacilityDtos;
using Domain.Common;

namespace Application.Abstractions.Interfaces
{
    public interface IFacilityRepository
    {
        Task <Result<List<FacilityTypeDto>>> GetFacilityTypesAsync();
    }
}
