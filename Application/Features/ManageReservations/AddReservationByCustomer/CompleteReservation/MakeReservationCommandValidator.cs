using System.Text.Json;
using Application.DTOs.ReservationDtos;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ManageReservations.AddReservationByCustomer.CompleteReservation;

public class MakeReservationCommandValidator : AbstractValidator<MakeReservationCommand>
{
    private const int MaxFileSize = 5 * 1024 * 1024; // 5mb in bytes
    private static readonly string[] AllowedFileTypes = [".jpg", ".jpeg", ".png", ".pdf"];

    public MakeReservationCommandValidator()
    {
        // validate facility id
        RuleFor(x => x.ReservationDto).NotNull().WithMessage("Reservation data is required");

        When(x => x.ReservationDto != null, () =>
        {
            RuleFor(x => x.ReservationDto.FacilityId)
                .GreaterThan(0)
                .WithMessage("Facility ID is required");

            // validate customer types
            RuleFor(x => x.ReservationDto.CustomerType)
                .NotEmpty()
                .WithMessage("Customer type is required")
                .Must(ValidCustomerType)
                .WithMessage("Invalid customer type. Must be 'Private', 'Public', or 'Corporate'");

            // validate start and end date
            RuleFor(x => x.ReservationDto.StartDate)
                .NotEmpty()
                .WithMessage("Start date is required")
                .Must(date => date > DateTime.UtcNow.AddHours(-1))
                .WithMessage("Start date must be in future");

            RuleFor(x => x.ReservationDto.EndDate)
                .NotEmpty()
                .WithMessage("End date is required")
                .Must(ValidDate)
                .WithMessage("End date is not valid")
                .GreaterThan(x => x.ReservationDto.StartDate)
                .WithMessage("End date must be after start date");

            RuleFor(x => x.ReservationDto.Packages)
                .NotEmpty()
                .WithMessage("At least one package must be selected");

            // validate packages
            RuleForEach(x => x.ReservationDto.Packages)
                .ChildRules(package =>
                {
                    package.RuleFor(p => p.PackageId)
                        .GreaterThan(0)
                        .WithMessage("Package ID must be greater than 0");

                    package.RuleFor(p => p.Quantity)
                        .GreaterThan(0)
                        .WithMessage("Package quantity must be greater than 0");
                });

            RuleFor(x => x.ReservationDto.PackagesJson)
                .NotEmpty().WithMessage("Package data is required")
                .Must(ValidPackageJson).WithMessage("Invalid package format");

            // validate payment details
            RuleFor(x => x.ReservationDto.PaymentMethod)
                .NotEmpty()
                .WithMessage("Payment method is required")
                .Must(ValidPaymentMethod)
                .WithMessage("Invalid payment method. Must be 'GoAndPay', 'BankTransfer', or 'Online'");

            When(x => x.ReservationDto.PaymentMethod == "BankTransfer" && x.ReservationDto.AmountPaid > 0, () =>
            {
                RuleFor(x => x.ReservationDto.BankTransferReceipts)
                    .NotEmpty()
                    .WithMessage("Bank transfer receipt is required when paying by bank transfer");
                        
                RuleForEach(x => x.ReservationDto.BankTransferReceipts)
                    .Must(ValidFileSize)
                    .WithMessage("Bank transfer receipt must be less than 5MB")
                    .Must(ValidFileType)
                    .WithMessage("Bank transfer receipt must be a JPG, PNG, or PDF file");
            });

            When(x => x.ReservationDto.PaymentMethod == "Online", () =>
            {
                RuleFor(x => x.ReservationDto.AmountPaid)
                    .GreaterThan(0)
                    .WithMessage("Payment amount must be greater than 0 for online payments");
            });

            // document validation for public and corporate customers
            When(x => x.ReservationDto.CustomerType != "Private", () =>
            {
                RuleFor(x => x.ReservationDto.Documents)
                    .NotEmpty()
                    .WithMessage("Supporting documents are required for public or corporate customers");
                        
                RuleForEach(x => x.ReservationDto.Documents)
                    .Must(ValidFileSize)
                    .WithMessage("Document must be less than 5MB")
                    .Must(ValidFileType)
                    .WithMessage("Document must be a JPG, PNG, or PDF file");
            });

            // validation for employee bookings
            When(x => x.AuthenticatedUserRole == "Employee", () =>
            {
                RuleFor(x => x.ReservationDto.GuestEmail)
                    .NotEmpty()
                    .WithMessage("Guest email is required for employee bookings")
                    .EmailAddress()
                    .WithMessage("Invalid email address format");

                RuleFor(x => x.ReservationDto.GuestPhone)
                    .NotEmpty()
                    .WithMessage("Guest phone is required for employee bookings");
            });
        });
    }

    // valid customer types
    private static bool ValidCustomerType(string customerType)
    {
        return new[] { "Private", "Public", "Corporate" }.Contains(customerType);
    }

    // valid payment methods
    private static bool ValidPaymentMethod(string paymentMethod)
    {
        return new[] { "GoAndPay", "BankTransfer", "Online" }.Contains(paymentMethod);
    }

    private static bool ValidDate(DateTime date)
    {
        return date != default && date > DateTime.Now;
    }

    private static bool ValidFileSize(IFormFile? file)
    {
        return file?.Length <= MaxFileSize;
    }

    private static bool ValidFileType(IFormFile? file)
    {
        if (file == null) return false;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return AllowedFileTypes.Contains(extension);
    }

    private static bool ValidPackageJson(string json)
    {
        try
        {
            var packages = JsonSerializer.Deserialize<List<SelectedPackageDto>>(json);

            return packages?.Any() == true;
        }
        catch
        {
            return false;
        }
    }
}