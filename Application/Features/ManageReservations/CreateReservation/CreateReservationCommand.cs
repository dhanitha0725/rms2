using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.CreateReservation;

public class CreateReservationCommand : IRequest<Result<ReservationResultDto>>
{
    public int FacilityId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Total { get; set; }
    public string CustomerType { get; set; }
    public List<BookingItemDto> Items { get; set; }
    public ReservationUserInfoDto UserDetails { get; set; }
}