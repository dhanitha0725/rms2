using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.AddReservationByCustomer.CompleteReservation;

public class MakeReservationCommand : IRequest<Result<MakeReservationDto>>
{
    public MakeReservationDto ReservationDto { get; set; }
    public int AuthenticatedUserId { get; set; }
    public string AuthenticatedUserRole { get; set; } //created by customer or employee
}