using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PlayerBankMap : IEntityTypeConfiguration<PlayerBank>
    {
        void IEntityTypeConfiguration<PlayerBank>.Configure(EntityTypeBuilder<PlayerBank> builder)
        {
            builder.ToTable("player_banks");

            builder.HasKey(c => c.Id)
                .HasName("pk_player_banks");

            builder.Property(c => c.PlayerCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(c => c.RealName)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(c => c.BankName)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(c => c.BankAccount)
                .HasMaxLength(32)
                .IsRequired();

            builder.HasOne(c => c.Player)
                .WithMany(c => c.Banks)
                .HasForeignKey(c => c.PlayerCode)
                .HasConstraintName("fk_player_banks_players_player_code");

            builder.HasIndex(c => c.PlayerCode)
                .HasDatabaseName("ix_player_bankers_player_code");
        }
    }
}