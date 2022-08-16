using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

internal class MarketingStatisticsSettingsMap : IEntityTypeConfiguration<MarketingStatisticsSettings>
{
    public void Configure(EntityTypeBuilder<MarketingStatisticsSettings> builder)
    {
        builder.ToTable("marketing_statistics_settings");

        builder.HasKey(b => b.RecordKey);

        builder.Property(b => b.RecordKey)
            .IsRequired();
        builder.Property(b => b.ProcessedTime)
            .IsRequired()
            .HasColumnType("timestamptz");
    }
}
