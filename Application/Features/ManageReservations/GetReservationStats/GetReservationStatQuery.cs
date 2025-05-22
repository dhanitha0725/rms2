using System;
using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.GetReservationStats;

public class GetReservationStatQuery : IRequest<Result<ReservationStatsDto>>
{
}
