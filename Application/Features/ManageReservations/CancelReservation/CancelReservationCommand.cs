using System;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.CancelReservation;

public class CancelReservationCommand : IRequest<Result<int>>
{
    public int ReservationId { get; set; }
}
