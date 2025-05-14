using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.Reports
{
    public class GetFinancialReportQueryHandler(
            IReportRepository reportRepository)
            : IRequestHandler<GetFinancialReportQuery, Result<List<FinancialReport>>>
    {
        public async Task<Result<List<FinancialReport>>> Handle(
            GetFinancialReportQuery request,
            CancellationToken cancellationToken)
        {
            var financialReport = await reportRepository.GetFinancialReportAsync(
                request.StartDate,
                request.EndDate,
                request.FacilityIds,
                cancellationToken);

            return Result<List<FinancialReport>>.Success(financialReport); 
        }
    }
}
