using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.Reports
{
    public class GetReservationReportQuery : IRequest<Result<List<ReservationReport>>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<int>? FacilityIds { get; set; }
    }
}
