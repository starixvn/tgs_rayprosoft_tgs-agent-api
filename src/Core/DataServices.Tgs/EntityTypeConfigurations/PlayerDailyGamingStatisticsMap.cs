using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations;

internal class PlayerDailyGamingStatisticsMap : IEntityTypeConfiguration<PlayerDailyGamingStatistics>
{
    public void Configure(EntityTypeBuilder<PlayerDailyGamingStatistics> builder)
    {
        builder.ToTable("player_daily_gaming_statistics");

        builder.HasKey(pgs => new { pgs.ReportDate, pgs.PlayerCode, pgs.GpCode, pgs.SubGpCode })
            .HasName("pk_player_daily_gaming_statistics");

        builder.Property(pgs => pgs.WinLoss)
            .IsRequired();
    }
}
