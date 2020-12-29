using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class LicenseeCurrencyMap : IEntityTypeConfiguration<LicenseeCurrency>
    {
        void IEntityTypeConfiguration<LicenseeCurrency>.Configure(EntityTypeBuilder<LicenseeCurrency> builder)
        {
            builder.ToTable("LicenseeCurrencies", "dbo");
            builder.HasKey(bc => new {bc.LicenseeId, bc.CurrencyCode});
            builder
                .HasOne(bc => bc.Licensee)
                .WithMany(c => c.Currencies)
                .HasForeignKey(bc => bc.LicenseeId);

            builder
                .HasOne(bc => bc.Currency)
                .WithMany(c => c.LicenseeCurrencies)
                .HasForeignKey(bc => bc.CurrencyCode);

            builder.Property(c => c.CurrencyCode)
                .HasColumnType(ColumnTypes.Nvarchar(16))
                .IsRequired();
        }
    }
}