using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class GameProviderBetLimitMap : IEntityTypeConfiguration<GameProviderBetLimit>
    {
        void IEntityTypeConfiguration<GameProviderBetLimit>.Configure(EntityTypeBuilder<GameProviderBetLimit> builder)
        {
            builder.ToTable("GameProviderBetLimits", "dbo");
            builder.HasKey(e => e.Id)
                .IsClustered(false);

            builder.Property(e => e.CurrencyCode)
                .HasColumnType(ColumnTypes.Nvarchar(16));

            builder.Property(e => e.MinBet)
                .HasColumnType(ColumnTypes.Int)
                .IsRequired();

            builder.Property(e => e.MaxBet)
                .HasColumnType(ColumnTypes.Int)
                .IsRequired();

            builder.Property(e => e.MaxBetPerMatch)
                .HasColumnType(ColumnTypes.Int);

            builder.Property(e => e.ExternalSetting)
                .HasColumnType(ColumnTypes.Nvarchar(255))
                .IsRequired();

            builder.HasOne(e => e.Currency)
                .WithMany(e => e.GameProviderBetLimits)
                .HasForeignKey(e => e.CurrencyCode);

            builder.HasOne(e => e.GameProvider)
                .WithMany(e => e.BetLimits)
                .HasForeignKey(e => e.GameProviderId);
        }
    }
}