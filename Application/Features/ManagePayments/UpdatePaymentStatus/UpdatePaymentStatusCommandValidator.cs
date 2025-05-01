using FluentValidation;

namespace Application.Features.ManagePayments.UpdatePaymentStatus
{
    public class UpdatePaymentStatusCommandValidator : AbstractValidator<UpdatePaymentStatusCommand>
    {
        public UpdatePaymentStatusCommandValidator()
        {
            RuleFor(x => x.MerchantId)
                .NotEmpty()
                .WithMessage("Merchant ID is required.");

            RuleFor(x => x.OrderId)
                .NotEmpty()
                .WithMessage("Order ID is required.");

            RuleFor(x => x.PaymentId)
                .NotEmpty()
                .WithMessage("Payment ID is required.");


            RuleFor(x => x.PayhereAmount)
                .NotEmpty()
                .WithMessage("Payhere Amount is required.");

            RuleFor(x => x.PayhereCurrency)
                .NotEmpty()
                .WithMessage("Payhere Currency is required.")
                .Length(3)
                .WithMessage("Payhere Currency must be exactly 3 characters long.");

            RuleFor(x => x.StatusCode)
                .InclusiveBetween(-3, 2)
                .WithMessage("Status Code must be between -3 and 2.");

            RuleFor(x => x.Md5Sig)
                .NotEmpty()
                .WithMessage("MD5 Signature is required.");
        }
    }
}
