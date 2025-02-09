using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.AddFacility.Commands;
using FluentValidation;

namespace Application.Features.ManageFacility.Commands
{
    public class AddFacilityCommandValidator : AbstractValidator<AddFacilityCommand>
    {
        public AddFacilityCommandValidator()
        {
            RuleFor(p => p.FacilityDto.FacilityName)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
            
            RuleFor(p => p.FacilityDto.FacilityType)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
            
            RuleFor(p => p.FacilityDto.Location)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
            
            RuleFor(p => p.FacilityDto.Description)
                .MaximumLength(200).WithMessage("{PropertyName} must not exceed 200 characters.");
        }
    }
}
