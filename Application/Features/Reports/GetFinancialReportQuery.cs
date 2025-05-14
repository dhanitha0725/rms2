using Application.DTOs.ReportsDto;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.Reports
{
    public class GetFinancialReportQuery : IRequest<Result<List<FinancialReport>>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<int>? FacilityIds { get; set; }
    }
}
