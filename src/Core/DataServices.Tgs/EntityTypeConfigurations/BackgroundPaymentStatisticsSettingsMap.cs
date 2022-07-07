using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

internal class BackgroundPaymentStatisticsSettingsMap : BaseConfiguration, IEntityTypeConfiguration<BackgroundPaymentStatisticsSettings>
{
    public void Configure(EntityTypeBuilder<BackgroundPaymentStatisticsSettings> builder)
    {
        builder.ToTable("background_payment_statistics_settings");

        builder.HasKey(b => b.RecordKey);

        builder.Property(b => b.RecordKey)
            .IsRequired();
        builder.Property(b => b.ProcessedTime)
            .IsRequired()
            .HasColumnType("timestamptz");
    }
}
