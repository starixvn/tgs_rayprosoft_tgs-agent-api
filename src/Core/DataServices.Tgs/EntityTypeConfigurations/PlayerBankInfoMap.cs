using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PlayerBankInfoMap : IEntityTypeConfiguration<PlayerBankInfo>
    {
        void IEntityTypeConfiguration<PlayerBankInfo>.Configure(EntityTypeBuilder<PlayerBankInfo> builder)
        {
            builder.ToTable("player_bank_infos");

            builder.HasKey(c => c.Id)
                .HasName("pk_player_bank_infos");

            builder.Property(c => c.PlayerCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(c => c.BrandId)
                .IsRequired();

            builder.Property(c => c.PaymentChannelId)
                .IsRequired();

            builder.Property(c => c.BankType)
                .IsRequired();

            builder.Property(c => c.Detail)
                .HasColumnType(ColumnTypes.Jsonb);

            builder.Property(c => c.CreatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(c => c.UpdatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .IsRequired();

            builder.Property(c => c.UpdatedOn)
                .IsRequired();

            builder.HasOne(c => c.Player)
                .WithMany(c => c.Banks)
                .HasForeignKey(c => c.PlayerCode)
                .HasConstraintName("fk_player_bank_infos_players_player_code");

            builder.HasIndex(c => c.PlayerCode)
                .HasDatabaseName("ix_player_bank_infos_player_code");

            builder.HasOne(c => c.Brand)
                .WithMany()
                .HasConstraintName("fk_player_bank_infos_brands_brand_id");

            builder.HasIndex(c => c.BrandId)
                .HasDatabaseName("ix_player_bank_infos_brand_id");

            builder.HasOne(c => c.PaymentChannel)
                .WithMany()
                .HasConstraintName("fk_player_bank_infos_payment_channels_payment_channel_id");

            builder.HasIndex(c => c.PaymentChannelId)
                .HasDatabaseName("ix_player_bank_infos_payment_channel_id");
        }
    }
}
