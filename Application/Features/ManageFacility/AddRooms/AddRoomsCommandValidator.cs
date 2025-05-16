using Application.Abstractions.Interfaces;
using Domain.Entities;
using FluentValidation;

namespace Application.Features.ManageFacility.AddRooms
{
    public class AddRoomsCommandValidator : AbstractValidator<AddRoomsCommand>
    {
        public AddRoomsCommandValidator(IGenericRepository<Facility, int> facilityRepo)
        {
            RuleFor(x => x.RoomConfigurationDto.Quantity)
                .GreaterThan(0).WithMessage("At least 1 room required");

            RuleFor(x => x.RoomConfigurationDto.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be positive");

            RuleFor(x => x.RoomConfigurationDto.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be positive");
        }
    }
}
