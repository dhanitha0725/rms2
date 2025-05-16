using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class ReservationReportConfiguration : IEntityTypeConfiguration<ReservationReport>
    {
        public void Configure(EntityTypeBuilder<ReservationReport> builder)
        {
            builder.HasNoKey();
        }
    }
}
