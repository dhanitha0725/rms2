using FluentValidation;

namespace Application.Features.ManageReservations.UploadDocument
{
    public class UploadDocumentsCommandValidator : AbstractValidator<UploadDocumentsCommand>
    {
        public UploadDocumentsCommandValidator()
        {
            RuleFor(x => x.Document)
                .NotNull()
                .WithMessage("Document is required.");

            RuleFor(x => x.Document.File)
                .NotNull()
                .WithMessage("File is required.")
                .Must(file => file.ContentType == "application/pdf")
                .WithMessage("Only PDF documents are allowed.")
                .Must(file => file.Length <= 5 * 1024 * 1024) // 5MB
                .WithMessage("File size must be less than or equal to 5MB.");
        }
    }
}
