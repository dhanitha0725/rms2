using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.CreateReservation;

public class CreateReservationCommand : IRequest<Result<ReservationResultDto>>
{
    public CreateReservationDto ReservationDto { get; set; }
}