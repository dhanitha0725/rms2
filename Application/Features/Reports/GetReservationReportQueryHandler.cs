using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.Reports
{
    public class GetReservationReportQueryHandler(
            IReportRepository reportRepository)
            : IRequestHandler<GetReservationReportQuery, Result<List<ReservationReport>>>
    {
        public async Task<Result<List<ReservationReport>>> Handle(
            GetReservationReportQuery request,
            CancellationToken cancellationToken)
        {
            var reservationReport = await reportRepository.GetReservationReportAsync(
                request.StartDate,
                request.EndDate,
                request.FacilityIds,
                cancellationToken);

            return Result<List<ReservationReport>>.Success(reservationReport);
        }
    }
 
}
