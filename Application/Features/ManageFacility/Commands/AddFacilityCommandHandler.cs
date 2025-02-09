using System.Text.Json;
using MediatR;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Serilog;

namespace Application.Features.AddFacility.Commands
{
    public class AddFacilityCommandHandler : 
        IRequestHandler<AddFacilityCommand, Result<int>>
    {
        private readonly IGenericRepository<Facility, int> _facilityRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
    

        public AddFacilityCommandHandler(
            IGenericRepository<Facility, int> facilityRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger logger)
        {
            _facilityRepository = facilityRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(
            AddFacilityCommand request, 
            CancellationToken cancellationToken)
        {
            var facility = _mapper.Map<Facility>(request.FacilityDto);

            facility.CreatedDate = DateTime.UtcNow;

            // serialize attributes
            facility.Attributes = JsonSerializer.Serialize(
                request.FacilityDto.Attributes);

            await _facilityRepository.AddAsync(facility, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.Information("New Facility Added with Id: {FacilityId}", facility.FacilityID);

            return Result<int>.Success(facility.FacilityID);
        }
    }
}
