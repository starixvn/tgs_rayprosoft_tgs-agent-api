using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class BetLimitMap : IEntityTypeConfiguration<BetLimit>
    {
        void IEntityTypeConfiguration<BetLimit>.Configure(EntityTypeBuilder<BetLimit> builder)
        {
            builder.ToTable("BetLimits", "dbo");
            builder.HasKey(b => b.Id)
                .IsClustered(false);

            builder.HasIndex(e => new {e.CurrencyCode, e.Id})
                .IsClustered();

            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd();

            builder.Property(b => b.CurrencyCode)
                .HasColumnType(ColumnTypes.Nvarchar(16))
                .IsRequired();

            builder.Property(b => b.MinBet)
                .HasColumnType(ColumnTypes.Int)
                .IsRequired();

            builder.Property(b => b.MaxBet)
                .HasColumnType(ColumnTypes.Int)
                .IsRequired();

            builder.Property(b => b.MaxBetPerMatch)
                .HasColumnType(ColumnTypes.Int)
                .IsRequired();

            builder.Property(b => b.CreatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(b => b.UpdatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(b => b.CreatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();

            builder.Property(b => b.UpdatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();

            builder.HasOne(e => e.Currency)
                .WithMany(e => e.BetLimits)
                .HasForeignKey(e => e.CurrencyCode);
        }
    }
}