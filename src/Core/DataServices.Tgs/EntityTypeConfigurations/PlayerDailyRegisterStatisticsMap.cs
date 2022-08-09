using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

internal class PlayerDailyRegisterStatisticsMap : IEntityTypeConfiguration<PlayerDailyRegisterStatistics>
{
    public void Configure(EntityTypeBuilder<PlayerDailyRegisterStatistics> builder)
    {
        builder.ToTable("player_daily_register_statistics");

        builder.HasKey(prs => new { prs.ReportDate, prs.PlayerCode });

        builder.Property(prs => prs.LastLoginOn)
            .HasColumnType("timestamptz")
            .IsRequired();
        builder.Property(prs => prs.DailyLoginCount)
            .IsRequired();
        builder.Property(prs => prs.TotalLoginCount)
            .IsRequired();
    }
}
