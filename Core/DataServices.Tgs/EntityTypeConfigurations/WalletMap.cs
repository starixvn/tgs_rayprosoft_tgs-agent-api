using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class WalletTypeMap : IEntityTypeConfiguration<Wallet>
    {
        void IEntityTypeConfiguration<Wallet>.Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.ToTable("Wallets", "dbo");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.PlayerCode)
                .HasColumnType(ColumnTypes.Varchar(16))
                .IsRequired();

            builder.Property(e => e.CurrencyCode)
                .HasColumnType(ColumnTypes.Nvarchar(16))
                .IsRequired();

            builder.Property(e => e.ExternalCurrencyCode)
                .HasColumnType(ColumnTypes.Nvarchar(16))
                .IsRequired();

            builder.Property(e => e.Balance)
                .HasColumnType(ColumnTypes.Decimal(18, 6))
                .IsRequired();

            builder.Property(e => e.CreatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(e => e.UpdatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.HasOne(e => e.Player)
                .WithMany(e => e.Wallets)
                .HasForeignKey(e => e.PlayerCode)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}