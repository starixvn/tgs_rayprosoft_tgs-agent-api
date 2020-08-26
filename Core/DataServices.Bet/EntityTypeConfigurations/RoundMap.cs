using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Bet.Models;

namespace TGS.Core.DataServices.Bet.EntityTypeConfigurations
{
    internal class RoundMap : IEntityTypeConfiguration<Round>
    {
        void IEntityTypeConfiguration<Round>.Configure(EntityTypeBuilder<Round> builder)
        {
            builder.ToTable("Rounds", "dbo");
            builder.HasKey(bc => bc.Id).IsClustered(false);

            builder.HasMany(p => p.GameActions)
                .WithOne(p => p.Round)
                .HasForeignKey(p => p.RoundId);

            builder.HasOne(p => p.RoundDetail)
                .WithOne(p => p.Round)
                .HasForeignKey<RoundDetail>(p => p.RoundId);

            builder.Property(c => c.Status)
                .HasColumnType("smallint")
                .IsRequired();

            builder.Property(c => c.GameCode)
                .HasColumnType("nvarchar(255)")
                .IsRequired()
                .IsUnicode();
            builder.Property(c => c.GameName)
                .HasColumnType("nvarchar(255)")
                .IsRequired()
                .IsUnicode();
            builder.Property(c => c.GameType)
                .HasColumnType("smallint")
                .IsRequired();

            builder.Property(c => c.GameProviderCode)
                .HasColumnType("nvarchar(255)")
                .IsRequired()
                .IsUnicode();
            builder.Property(c => c.GameProviderName)
                .HasColumnType("nvarchar(255)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.GameProviderName)
                .HasColumnType("nvarchar(255)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.PlayerCode)
                .HasColumnType("varchar(16)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.PlayerTypes)
                .HasColumnType("smallint")
                .IsRequired();

            builder.Property(c => c.PlatformType)
                .HasColumnType("smallint")
                .IsRequired();

            builder.Property(c => c.CurrencyCode)
                .HasColumnType("nvarchar(10)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.GameProviderConfiguration)
                .HasColumnType("nvarchar(4000)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.GameProviderConfigurationId)
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.Property(c => c.WalletId)
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.Property(c => c.GameId)
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.Property(c => c.BrandId)
                .HasColumnType("uniqueidentifier")
                .IsRequired(false);

            builder.Property(c => c.ExternalRoundId)
                .HasColumnType("nvarchar(255)")
                .IsRequired(false);

            builder.Property(c => c.CreatedOn)
                .HasColumnType("datetimeoffset(7)")
                .IsRequired();

            builder.Property(c => c.ClosedOn)
                .HasColumnType("datetimeoffset(7)")
                .IsRequired(false);

            builder.Property(c => c.UpdatedOn)
                .HasColumnType("datetimeoffset(7)")
                .IsRequired();

            builder.Property(c => c.AggregatedOn)
                .HasColumnType("datetimeoffset(7)")
                .IsRequired(false);

            builder.Property(c => c.ExternalCreatedOn)
                .HasColumnType("datetimeoffset(7)")
                .IsRequired(false);

            builder.Property(c => c.ExternalClosedOn)
                .HasColumnType("datetimeoffset(7)")
                .IsRequired(false);

            builder.Property(c => c.ExternalAggregatedOn)
                .HasColumnType("datetimeoffset(7)")
                .IsRequired(false);

            builder.Property(c => c.BetAmount)
                .HasColumnType("decimal(18, 6)")
                .IsRequired(false);

            builder.Property(c => c.GrossGamingRevenue)
                .HasColumnType("decimal(18, 6)")
                .IsRequired(false);

            builder.Property(c => c.TiebetAdjustment)
                .HasColumnType("decimal(18, 6)")
                .IsRequired(false);

            builder.Property(c => c.Commission)
                .HasColumnType("decimal(18, 6)")
                .IsRequired(false);
        }
    }
}
