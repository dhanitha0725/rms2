using System;
using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.GetFacilityChartData
{
    public class GetFacilityChartDataQuery : IRequest<Result<FacilityReservationCountsResponse>>
    {
    }
}
