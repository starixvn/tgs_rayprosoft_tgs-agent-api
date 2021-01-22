using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PlayerLoginRecordMap : IEntityTypeConfiguration<PlayerLoginRecord>
    {
        void IEntityTypeConfiguration<PlayerLoginRecord>.Configure(EntityTypeBuilder<PlayerLoginRecord> builder)
        {
            builder.ToTable("player_login_records");

            builder.HasKey(b => b.Id)
                .HasName("pk_player_login_records");

            builder.Property(b => b.PlayerCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(b => b.BrandId)
                .IsRequired();

            builder.Property(b => b.Status)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(b => b.PlatformType)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(b => b.CreatedOn)
                .IsRequired();

            builder.Property(b => b.ExpiredOn)
                .IsRequired();

            builder.HasOne(b => b.Player)
                .WithMany(b => b.PlayerLoginRecords)
                .HasForeignKey(b => b.PlayerCode)
                .HasConstraintName("fk_player_login_records_players_player_code")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(b => b.PlayerCode)
                .HasDatabaseName("ix_player_login_records_player_code");

            builder.HasOne(b => b.Brand)
                .WithMany(b => b.PlayerLoginRecords)
                .HasForeignKey(b => b.BrandId)
                .HasConstraintName("fk_player_login_records_brands_brand_id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(b => b.BrandId)
                .HasDatabaseName("ix_player_login_records_brand_id");
        }
    }
}