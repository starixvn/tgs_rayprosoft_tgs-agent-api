using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class BetLimitMap : IEntityTypeConfiguration<BetLimit>
    {
        void IEntityTypeConfiguration<BetLimit>.Configure(EntityTypeBuilder<BetLimit> builder)
        {
            builder.ToTable("bet_limits");

            builder.HasKey(b => b.Id)
                .HasName("pk_bet_limits");

            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd();

            builder.Property(b => b.CurrencyCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(b => b.MinBet)
                .IsRequired();

            builder.Property(b => b.MaxBet)
                .IsRequired();

            builder.Property(b => b.MaxBetPerMatch)
                .IsRequired();

            builder.Property(b => b.CreatedOn)
                .IsRequired();

            builder.Property(b => b.UpdatedOn)
                .IsRequired();

            builder.Property(b => b.CreatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(b => b.UpdatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.HasOne(e => e.Currency)
                .WithMany(e => e.BetLimits)
                .HasForeignKey(e => e.CurrencyCode)
                .HasConstraintName("fk_bet_limits_currencies_currency_code");

            builder.HasIndex(e => e.CurrencyCode)
                .HasDatabaseName("ix_bet_limits_currency_code");
        }
    }
}