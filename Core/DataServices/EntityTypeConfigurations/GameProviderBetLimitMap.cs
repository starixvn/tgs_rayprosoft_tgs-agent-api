using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class GameProviderBetLimitMap : IEntityTypeConfiguration<GameProviderBetLimit>
    {
        void IEntityTypeConfiguration<GameProviderBetLimit>.Configure(EntityTypeBuilder<GameProviderBetLimit> builder)
        {
            builder.ToTable("GameProviderBetLimits", "dbo");
            builder.HasKey(b => b.Id)
                .IsClustered(false);

            builder.Property(b => b.Code)
                .HasColumnType(ColumnTypes.Nvarchar(255))
                .IsRequired();

            builder.Property(b => b.BetLimitGroupId)
                .IsRequired();

            builder.Property(b => b.CurrencyCode)
                .HasColumnType(ColumnTypes.Nvarchar(16));

            builder.HasOne(b => b.Currency)
                .WithMany(b => b.GameProviderBetLimits)
                .HasForeignKey(b => b.CurrencyCode);

            builder.HasOne(b => b.GameProvider)
                .WithMany(b => b.GameProviderBetLimits)
                .HasForeignKey(b => b.GameProviderId);

            builder.HasOne(b => b.BetLimitGroup)
                .WithMany(b => b.GameProviderBetLimits)
                .HasForeignKey(b => b.BetLimitGroupId);
        }
    }
}
