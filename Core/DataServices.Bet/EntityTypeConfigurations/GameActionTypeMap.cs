using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Bet.Models;

namespace TGS.Core.DataServices.Bet.EntityTypeConfigurations
{
    internal class GameActionTypeMap : IEntityTypeConfiguration<GameAction>
    {
        void IEntityTypeConfiguration<GameAction>.Configure(EntityTypeBuilder<GameAction> builder)
        {
            builder.ToTable("GameActions", "dbo");
            builder.HasKey(bc => bc.Id).IsClustered(false);

            builder.Property(c => c.ExternalBetId)
                .HasColumnType("nvarchar(256)")
                .IsRequired();

            builder.Property(c => c.BetId)
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.HasMany(p => p.FollowUpGameActions)
                .WithOne(p => p.ReferencedGameAction)
                .HasForeignKey(p => p.ReferencedGameActionId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Property(c => c.IsClosingRound)
                .IsRequired();

            builder.Property(c => c.ClosingRoundExplicitRequest)
                .IsRequired(false);

            builder.Property(c => c.ExternalTransactionId)
                .HasColumnType("nvarchar(128)")
                .IsRequired(false);

            builder.Property(c => c.Type)
                .HasColumnType("smallint")
                .IsRequired();

            builder.Property(c => c.RoundId)
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.Property(c => c.WalletId)
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.Property(c => c.TokenId)
                .HasColumnType("uniqueidentifier")
                .IsRequired(false);

            builder.Property(c => c.GameActionBatchId)
                .HasColumnType("uniqueidentifier")
                .IsRequired(false);

            builder.Property(c => c.ReferencedGameActionId)
                .HasColumnType("uniqueidentifier")
                .IsRequired(false);

            builder.Property(c => c.GameProviderConfigurationId)
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.Property(c => c.BrandId)
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.Property(c => c.BetId)
                .HasColumnType("uniqueidentifier")
                .IsRequired(false);


            builder.Property(c => c.GameActionGroupId)
                .HasColumnType("uniqueidentifier")
                .IsRequired(false);

            builder.Property(c => c.RecordedOn)
                .HasColumnType("datetimeoffset(7)")
                .IsRequired();

            builder.Property(c => c.SentOn)
                .HasColumnType("datetimeoffset(7)")
                .IsRequired(false);

            builder.Property(c => c.StoredOn)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(c => c.Turnover)
                .HasColumnType("decimal(18, 6)")
                .IsRequired();

            builder.Property(c => c.GrossGamingRevenue)
                .HasColumnType("decimal(18, 6)")
                .IsRequired();

            builder.Property(c => c.UnsettledBets)
                .HasColumnType("decimal(18, 6)")
                .IsRequired();

            builder.Property(c => c.Amount)
                .HasColumnType("decimal(18, 6)")
                .IsRequired();

            builder.Property(c => c.Balance)
                .HasColumnType("decimal(18, 6)")
                .IsRequired(false);

            builder.Property(c => c.PlayerIp)
                .HasColumnType("varchar(128)")
                .IsRequired();

            builder.Property(c => c.ExternalGameActionGroupId)
                .HasColumnType("nvarchar(128)")
                .IsRequired();

            builder.Property(c => c.BetType)
                .HasColumnType("nvarchar(2000)")
                .IsRequired();

            builder.Property(c => c.Description)
                .HasColumnType("nvarchar(4000)")
                .IsRequired();

            builder.Property(c => c.GameProviderConfiguration)
                .HasColumnType("nvarchar(4000)")
                .IsRequired();
        }
    }
}
