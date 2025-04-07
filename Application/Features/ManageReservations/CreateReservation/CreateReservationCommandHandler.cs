using Application.Abstractions.Interfaces;
using Application.DTOs.ReservationDtos;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Serilog;

namespace Application.Features.ManageReservations.CreateReservation
{
    public class CreateReservationCommandHandler(
                IGenericRepository<User, int> userRepository,
                IGenericRepository<Reservation, int> reservationRepository,
                IUnitOfWork unitOfWork,
                ILogger logger)
                : IRequestHandler<CreateReservationCommand, Result<ReservationResultDto>>
    {
        public async Task<Result<ReservationResultDto>> Handle(
            CreateReservationCommand request,
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var user = await userRepository.GetByIdAsync(request.ReservationDto.UserId, cancellationToken);
                if (user == null)
                {
                    return Result<ReservationResultDto>.Failure(new Error("User not found"));
                }

                // create reservation
                var reservation = new Reservation
                {
                    UserID = request.ReservationDto.UserId,
                    StartDate = request.ReservationDto.StartDate,
                    EndDate = request.ReservationDto.EndDate,
                    Status = ReservationStatus.PaymentPending,
                    UserType = request.ReservationDto.UserType,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = request.ReservationDto.CreatedBy,
                    UpdatedBy = null,
                    ReservationUserDetail = new ReservationUserDetail
                    {
                        FirstName = request.ReservationDto.FirstName,
                        LastName = request.ReservationDto.LastName,
                        Email = request.ReservationDto.Email,
                        PhoneNumber = request.ReservationDto.PhoneNumber,
                        OrganizationName = request.ReservationDto.OrganizationName
                    }
                };

                await reservationRepository.AddAsync(reservation, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                // add reserved packages
                foreach (var package in request.ReservationDto.Packages)
                {
                    reservation.ReservedPackages.Add(new ReservedPackage
                    {
                        ReservationID = reservation.ReservationID,
                        PackageID = package.PackageId,
                        status = "Reserved"
                    });
                }

                await unitOfWork.CommitTransactionAsync(cancellationToken);

                var resultDto = new ReservationResultDto
                {
                    ReservationId = reservation.ReservationID,
                    Status = reservation.Status.ToString()
                };

                return Result<ReservationResultDto>.Success(resultDto);
            }
            catch (Exception e)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.Error(e, "Error creating reservation");
                throw;
            }
        }
    }
}
