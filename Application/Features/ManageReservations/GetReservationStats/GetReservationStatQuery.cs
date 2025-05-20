using System;
using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.GetReservationStats;

public class GetReservationStatQuery : IRequest<Result<ReservationStatsDto>>
{
    // Optional date range filter
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Optional facility filter
    public int? FacilityId { get; set; }
}
