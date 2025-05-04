using Application.Abstractions.Interfaces;
using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.GetReservationDetails
{
    public class GetReservationDetailsQueryHandler(
            IReservationRepository reservationRepository)
            : IRequestHandler<GetReservationDetailsQuery, Result<ReservationDetailsDto>>
    {
        public async Task<Result<ReservationDetailsDto>> Handle(
            GetReservationDetailsQuery request,
            CancellationToken cancellationToken)
        {
            // get reservation details
            var reservation = await reservationRepository.GetReservationDetailsAsync(
                    request.ReservationId, cancellationToken);

            // check if reservation exists
            if (reservation == null)
            {
                return Result<ReservationDetailsDto>.Failure(new Error("Reservation not found."));
            }

            // map reservation to ReservationDetailsDto
            var result = new ReservationDetailsDto
            {
                ReservationId = reservation.ReservationID,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                CreatedDate = reservation.CreatedDate,
                UpdatedDate = reservation.UpdatedDate,
                Total = reservation.Total,
                Status = reservation.Status.ToString(),
                UserType = reservation.UserType,
                ReservationUser = new ReservationUserDetailDto
                {
                    FirstName = reservation.ReservationUserDetail.FirstName,
                    LastName = reservation.ReservationUserDetail.LastName,
                    Email = reservation.ReservationUserDetail.Email,
                    PhoneNumber = reservation.ReservationUserDetail.PhoneNumber,
                    OrganizationName = reservation.ReservationUserDetail.OrganizationName
                },
                Payments = reservation.Payments.Select(p => new PaymentDto
                {
                    OrderID = p.OrderID,
                    Method = p.Method,
                    AmountPaid = p.AmountPaid,
                    CreatedDate = p.CreatedDate,
                    Status = p.Status,
                }).ToList(),
                ReservedPackages = reservation.ReservedPackages.Select(rp => new ReservationPackageDto
                {
                    PackageName = rp.Package.PackageName,
                    FacilityName = rp.Package.Facility.FacilityName
                }).ToList(),
                ReservedRooms = reservation.ReservedRooms.Select(rr => new ReservationRoomDto
                {
                    RoomType = rr.Room.Type,
                    FacilityName = rr.Room.Facility.FacilityName
                }).ToList()
            };

            return Result<ReservationDetailsDto>.Success(result);
        }
    }
}
