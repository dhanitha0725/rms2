using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class FinancialReportConfiguration : IEntityTypeConfiguration<FinancialReport>
    {
        public void Configure(EntityTypeBuilder<FinancialReport> builder)
        {
            builder.HasNoKey();
        }
    }
}
