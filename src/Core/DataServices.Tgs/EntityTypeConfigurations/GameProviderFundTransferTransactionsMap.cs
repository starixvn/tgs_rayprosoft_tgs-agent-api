using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class GameProviderFundTransferTransactionsMap : IEntityTypeConfiguration<GameProviderFundTransferTransaction>
    {
        void IEntityTypeConfiguration<GameProviderFundTransferTransaction>.Configure(EntityTypeBuilder<GameProviderFundTransferTransaction> builder)
        {
            builder.ToTable("game_provider_fund_transfer_transactions");

            builder.HasKey(e => e.GameProviderFundTransferTransactionId)
                .HasName("pk_game_provider_fund_transfer_transaction_id");

            builder.Property(c => c.TransactionId);

            builder.Property(c => c.GameProviderId)
                .IsRequired();

            builder.Property(c => c.WalletId)
                .IsRequired();

            builder.Property(c => c.GameProviderTransactionId);

            builder.Property(c => c.PlayerCode)
                .IsRequired();

            builder.Property(c => c.FundSource)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.FromWallet)
                .IsRequired();

            builder.Property(c => c.ToWallet)
                .IsRequired();

            builder.Property(e => e.Status)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.Amount)
                .HasPrecision(18, 6);

            builder.Property(c => c.Balance)
                .HasPrecision(18, 6);

            builder.Property(c => c.CreatedOn)
                .IsRequired();

            builder.Property(c => c.UpdatedOn)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(256);

            builder.HasIndex(e => e.TransactionId)
              .HasDatabaseName("ix_game_provider_fund_transfer_transactions_transaction_id");

            builder.HasIndex(e => e.WalletId)
                .HasDatabaseName("ix_game_provider_fund_transfer_transactions_wallet_id");

            builder.HasIndex(e => e.PlayerCode)
                .HasDatabaseName("ix_game_provider_fund_transfer_transactions_player_code");
        }
    }
}
