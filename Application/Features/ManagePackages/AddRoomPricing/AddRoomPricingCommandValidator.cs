using Application.Abstractions.Interfaces;
using Domain.Entities;
using FluentValidation;

namespace Application.Features.ManagePackages.AddRoomPricing
{
    public class AddRoomPricingCommandValidator : AbstractValidator<AddRoomPricingCommand>
    {
        public AddRoomPricingCommandValidator(
            IGenericRepository<Room, int> roomRepository,
            IGenericRepository<Facility, int> facilityRepository,
            IGenericRepository<RoomType, int> roomTypeRepository)
        {
            // Validate facility exists
            RuleFor(x => x.FacilityId)
                .NotEmpty().WithMessage("Facility ID is required.")
                .MustAsync(async (id, cancellation) =>
                    await facilityRepository.ExistsAsync(f => f.FacilityID == id, cancellation))
                .WithMessage(x => $"Facility with ID {x.FacilityId} does not exist.");

            // Validate room type exists
            RuleFor(x => x.RoomTypeId)
                .NotEmpty().WithMessage("Room type ID is required.")
                .MustAsync(async (id, cancellation) =>
                    await roomTypeRepository.ExistsAsync(rt => rt.RoomTypeID == id, cancellation))
                .WithMessage(x => $"Room type with ID {x.RoomTypeId} does not exist.");

            // Validate that at least one room of this type exists in the facility
            RuleFor(x => x)
                .MustAsync(async (cmd, cancellation) =>
                {
                    var roomsExist = await roomRepository.ExistsAsync(
                        r => r.FacilityID == cmd.FacilityId && r.RoomTypeID == cmd.RoomTypeId,
                        cancellation);
                    return roomsExist;
                })
                .WithMessage(x => $"No rooms of type ID {x.RoomTypeId} exist in facility ID {x.FacilityId}. " +
                                  "Please add rooms of this type to the facility before setting pricing.");

            // Validate pricing data
            RuleFor(x => x.Pricings)
                .NotEmpty().WithMessage("At least one pricing sector is required.")
                .Must(p => p.Values.All(v => v > 0))
                .WithMessage("All prices must be positive.");

            // Validate sectors
            RuleForEach(x => x.Pricings.Keys)
                .Must(sector => sector == "private" || sector == "public" || sector == "corporate")
                .WithMessage("Invalid sector name. Valid values are: private, public, corporate");
        }
    }
}
