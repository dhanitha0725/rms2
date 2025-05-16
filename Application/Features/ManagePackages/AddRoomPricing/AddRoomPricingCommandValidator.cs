    using FluentValidation;

namespace Application.Features.ManagePackages.AddRoomPricing
{
    public class AddRoomPricingCommandValidator : AbstractValidator<AddRoomPricingCommand>
    {

        public AddRoomPricingCommandValidator()
        {
            RuleFor(x => x.Pricings)
                .NotEmpty().WithMessage("At least one pricing sector is required.")
                .Must(p => p.Values.All(v => v > 0))
                .WithMessage("All prices must be positive.");
        }
    }
}
