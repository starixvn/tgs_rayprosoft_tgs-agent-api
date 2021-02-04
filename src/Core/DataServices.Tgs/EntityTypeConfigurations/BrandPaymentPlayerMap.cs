using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    public class BrandPaymentPlayerMap : IEntityTypeConfiguration<BrandPaymentPlayer>
    {
        void IEntityTypeConfiguration<BrandPaymentPlayer>.Configure(
            EntityTypeBuilder<BrandPaymentPlayer> builder)
        {
            builder.ToTable("brand_payment_players");

            builder.HasKey(e => new {e.BrandPaymentId, e.PlayerCode})
                .HasName("pk_brand_payment_players");

            builder.Property(p => p.ListType)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.HasOne(e => e.BrandPayment)
                .WithMany(e => e.BrandPaymentPlayers)
                .HasForeignKey(e => e.BrandPaymentId)
                .HasConstraintName("fk_brand_payment_players_brand_payments_brand_payment_id");

            builder.HasOne(e => e.Player)
                .WithMany(e => e.BrandPaymentPlayers)
                .HasForeignKey(e => e.PlayerCode)
                .HasConstraintName("fk_brand_payment_players_players_player_code");

            builder.HasIndex(e => e.PlayerCode)
                .HasDatabaseName("ix_brand_payment_players_player_code");
        }
    }
}