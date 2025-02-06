using MediatR;
using Application.Abstractions.Interfaces;
using Application.DTOs;
using AutoMapper;
using Domain.Common;
using Domain.Entities;

namespace Application.Features.AddFacility.Commands
{
    public class AddFacilityCommandHandler
            : IRequestHandler<AddFacilityCommand,
            Result<FacilityResponseDto>>
    {
        public Task<Result<FacilityResponseDto>> Handle(AddFacilityCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
