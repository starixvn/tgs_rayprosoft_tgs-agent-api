using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    class PlayerIpRecordMap : IEntityTypeConfiguration<PlayerIpRecord>
    {
        public void Configure(EntityTypeBuilder<PlayerIpRecord> builder)
        {
            builder.ToTable("player_ip_records");

            builder.HasKey(b => new { b.PlayerCode, b.IpType })
                .HasName("pk_player_ip_records");

            builder.Property(b => b.PlayerCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(b => b.IpType)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(b => b.Ip)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(b => b.RecordOn)
                .IsRequired()
                .HasDefaultValueSql("Now()");

            builder.HasOne<Player>()
                .WithMany()
                .HasForeignKey(b => b.PlayerCode)
                .HasConstraintName("fk_player_ip_records_players_player_code")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
