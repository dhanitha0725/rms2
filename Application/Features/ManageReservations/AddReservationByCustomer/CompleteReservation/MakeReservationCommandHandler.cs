using Application.Abstractions.Interfaces;
using Application.DTOs.ReservationDtos;
using Application.Features.ManageReservations.AddReservationByCustomer.CalculatePrice;
using Application.Features.ManageReservations.AddReservationByCustomer.CheckAvailability;
using Application.Features.ManageReservations.AddReservationByCustomer.CreateGuestUser;
using Application.Features.ManageReservations.AddReservationByCustomer.CreateReservation;
using Application.Features.ManageReservations.AddReservationByCustomer.ProcessPayment;
using Application.Features.ManageReservations.AddReservationByCustomer.UploadDocuments;
using Domain.Common;
using MediatR;
using Serilog;

namespace Application.Features.ManageReservations.AddReservationByCustomer.CompleteReservation;

public class MakeReservationCommandHandler(
    IMediator mediator,
    IUnitOfWork unitOfWork,
    ILogger logger)
    : IRequestHandler<MakeReservationCommand, Result<MakeReservationDto>>
{
    public async Task<Result<MakeReservationDto>> Handle(
        MakeReservationCommand request,
        CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var dto = request.ReservationDto;

            // Determine if the user is an employee or customer
            int userId;
            if (request.AuthenticatedUserRole == "Employee" && dto is { GuestEmail: not null, GuestPhone: not null })
            {
                var guestResult = await mediator.Send(new CreateGuestUserCommand
                {
                    GuestEmail = dto.GuestEmail,
                    GuestPhone = dto.GuestPhone
                }, cancellationToken);

                if (!guestResult.IsSuccess)
                {
                    logger.Error($"Failed to add guest: {guestResult.Error}");
                    return Result<MakeReservationDto>.Failure(new Error("Failed to add guest."));
                }

                userId = guestResult.Value;
            }
            else if (request.AuthenticatedUserRole == "Customer")
            {
                userId = request.AuthenticatedUserId;
            }
            else
            {
                logger.Error("Invalid user role or missing guest details");
                return Result<MakeReservationDto>.Failure(new Error("Invalid user role or missing guest details"));
            }

            // Check availability
            var availabilityResult = await mediator.Send(new CheckAvailabilityQuery
            {
                FacilityID = dto.FacilityId,
                Packages = dto.Packages,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            }, cancellationToken);

            if (!availabilityResult.IsSuccess)
            {
                logger.Error($"Error with checking availability: {availabilityResult.Error}");
                return Result<MakeReservationDto>.Failure(new Error("Error with checking availability."));
            }

            // Calculate price
            var priceResult = await mediator.Send(new CalculatePriceCommand
            {
                CustomerType = dto.CustomerType,
                Packages = dto.Packages,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            }, cancellationToken);

            if (!priceResult.IsSuccess)
            {
                logger.Error($"Error with calculating price: {priceResult.Error}");
                return Result<MakeReservationDto>.Failure(new Error("Error with calculating price."));
            }

            // Create reservation
            var reservationResult = await mediator.Send(new CreateReservationCommand
            {
                UserId = userId,
                FacilityId = dto.FacilityId,
                Packages = dto.Packages,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Total = priceResult.Value,
                CustomerType = dto.CustomerType,
                CreatedBy = request.AuthenticatedUserRole == "Employee" ? "Employee" : "Customer"
            }, cancellationToken);

            if (!reservationResult.IsSuccess)
            {
                logger.Error($"Error with creating reservation: {reservationResult.Error}");
                return Result<MakeReservationDto>.Failure(new Error("Error with creating reservation."));
            }

            int reservationId = reservationResult.Value;

            // Upload documents (if customer == public or corporate)
            if (dto.CustomerType != "Private" && dto.Documents?.Count > 0)
            {
                var uploadResult = await mediator.Send(new UploadDocumentsCommand
                {
                    ReservationID = reservationId,
                    UserID = userId,
                    Documents = dto.Documents
                }, cancellationToken);
                if (!uploadResult.IsSuccess)
                {
                    logger.Error($"Error with uploading documents: {uploadResult.Error}");
                    return Result<MakeReservationDto>.Failure(new Error("Error with uploading documents."));
                }
            }

            //process payment
            if (dto.CustomerType == "Private" || dto.AmountPaid > 0)
            {
                var paymentResult = await mediator.Send(new ProcessPaymentCommand
                {
                    ReservationId = reservationId,
                    UserId = userId,
                    PaymentMethod = dto.PaymentMethod,
                    AmountPaid = dto.AmountPaid,
                    BankTransferReceipts = dto.BankTransferReceipts
                }, cancellationToken);
                if (!paymentResult.IsSuccess)
                {
                    logger.Error($"Error with processing payment: {paymentResult.Error}");
                    return Result<MakeReservationDto>.Failure(new Error("Error with processing payment."));
                }
            }

            await unitOfWork.CommitTransactionAsync(cancellationToken);

            logger.Information($"Reservation created successfully: {reservationId}");

            return Result<MakeReservationDto>.Success(new MakeReservationDto());
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            logger.Error(e, "Error creating reservation");
            throw;
        }
    }
}