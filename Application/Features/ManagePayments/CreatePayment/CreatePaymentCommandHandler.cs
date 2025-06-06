﻿using Application.Abstractions.Interfaces;
using Application.DTOs.Payment;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Serilog;

namespace Application.Features.ManagePayments.CreatePayment
{
    public class CreatePaymentCommandHandler(
        IPayhereService payhereService,
        IUnitOfWork unitOfWork,
        ILogger logger,
        IGenericRepository<Payment, Guid> paymentRepository,
        IGenericRepository<ReservationUserDetail, int> reservationUseRepository) 
        : IRequestHandler<CreatePaymentCommand, Result<PaymentInitiationResponse>>
    {
        public async Task<Result<PaymentInitiationResponse>> Handle(
            CreatePaymentCommand request, 
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // Get all reservation user details and filter by the ReservationID
                var allUserDetails = await reservationUseRepository.GetAllAsync(cancellationToken);
                var reservationUserDetail = allUserDetails.FirstOrDefault(
                    ud => ud.ReservationID == request.ReservationId);
                if (reservationUserDetail == null)
                {
                    return Result<PaymentInitiationResponse>.Failure(new Error("Reservation user detail not found."));
                }

                var payment = new Payment
                {
                    OrderID = request.OrderId,
                    AmountPaid = request.Amount,
                    Currency = request.Currency,
                    CreatedDate = DateTime.UtcNow,
                    Status = "Pending",
                    Method = nameof(PaymentMethods.Online),
                    ReservationID = request.ReservationId,
                    ReservationUserID = reservationUserDetail.ReservationUserDetailID,
                };

                await paymentRepository.AddAsync(payment, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var paymentRequest = new PaymentRequest(
                    request.OrderId,
                    request.Amount,
                    request.Currency,
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.Phone,
                    request.Address,
                    request.City,
                    request.Items);

                var paymentCheckout = payhereService.PrepareCheckout(paymentRequest);


                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<PaymentInitiationResponse>.Success(paymentCheckout);
            }
            catch (Exception e)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.Error("Payment Failed", e);
                throw;
            }
        }
    }
}
