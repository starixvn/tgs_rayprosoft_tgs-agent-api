using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    public class WalletTypeConfiguration : IEntityTypeConfiguration<Wallet>
    {
        void IEntityTypeConfiguration<Wallet>.Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.ToTable("Wallets", "dbo");
            builder.HasKey(bc => bc.Id);

            builder.Property(c => c.PlayerCode)
                .HasColumnType("varchar(16)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.CurrencyCode)
                .HasColumnType("nvarchar(10)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.ExternalCurrencyCode)
                .HasColumnType("nvarchar(10)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Balance)
                .HasColumnType("decimal(18,6)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.CreatedOn)
                .HasColumnType("nvarchar(32)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.LastModifiedOn)
                .HasColumnType("nvarchar(32)")
                .IsRequired()
                .IsUnicode();
        }
    }
}
