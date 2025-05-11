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
            // Get reservation details
            var reservation = await reservationRepository.GetReservationDetailsAsync(
                request.ReservationId, cancellationToken);

            // Check if reservation exists
            if (reservation == null)
            {
                return Result<ReservationDetailsDto>.Failure(new Error("Reservation not found."));
            }

            // Map reservation to ReservationDetailsDto
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
                Payments = reservation.Payments?.Select(p => new PaymentDto
                {
                    OrderID = p.OrderID,
                    Method = p.Method,
                    AmountPaid = p.AmountPaid,
                    CreatedDate = p.CreatedDate,
                    Status = p.Status,
                }).ToList() ?? new List<PaymentDto>(),
                ReservedPackages = reservation.ReservedPackages?.Select(rp => new ReservationPackageDto
                {
                    PackageName = rp.Package.PackageName,
                    FacilityName = rp.Package.Facility.FacilityName
                }).ToList() ?? new List<ReservationPackageDto>(),
                ReservedRooms = reservation.ReservedRooms?
                    .GroupBy(rr => new
                    {
                        rr.Room.RoomType.TypeName,
                        rr.Room.Facility.FacilityName
                    }).Select(group => new ReservationRoomDto
                    {
                        RoomType = group.Key.TypeName,
                        FacilityName = group.Key.FacilityName,
                        Quantity = group.Count()
                    }).ToList() ?? new List<ReservationRoomDto>(),
                Documents = reservation.Documents?.Select(d => new DocumentDetailsDto()
                {
                    DocumentId = d.DocumentID,
                    DocumentType = d.DocumentType,
                    Url = d.Url
                }).ToList() ?? new List<DocumentDetailsDto>() 
            };

            // Add payment-related documents
            if (reservation.Payments != null)
            {
                foreach (var payment in reservation.Payments)
                {
                    if (payment.Documents != null)
                    {
                        result.Documents.AddRange(payment.Documents.Select(d => new DocumentDetailsDto()
                        {
                            DocumentId = d.DocumentID,
                            DocumentType = d.DocumentType,
                            Url = d.Url
                        }));
                    }
                }
            }

            return Result<ReservationDetailsDto>.Success(result);
        }
    }
}
