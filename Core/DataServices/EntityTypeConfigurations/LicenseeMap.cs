using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class LicenseeMap : BaseConfiguration, IEntityTypeConfiguration<Licensee>
    {
        void IEntityTypeConfiguration<Licensee>.Configure(EntityTypeBuilder<Licensee> builder)
        {
            builder.ToTable("Licensees", "dbo");
            BaseConfigureProperties(builder);

            builder.HasOne(p => p.Reseller)
                .WithMany(p => p.Licensees)
                .HasForeignKey(p => p.ResellerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.Status)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasColumnType(ColumnTypes.Nvarchar(256));

            builder.Property(c => c.ClientId)
                .HasColumnType(ColumnTypes.Varchar(128))
                .IsRequired();

            builder.Property(c => c.SecretKey)
                .HasColumnType(ColumnTypes.Varchar(255))
                .IsRequired(); ;

            builder.Property(c => c.TimeZoneId)
                .HasColumnType(ColumnTypes.Nvarchar(256));

            builder.Property(c => c.LoginUrl)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.CashierUrl)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.HelpUrl)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.TermsUrl)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.MobileLoginUrl)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.MobileCashierUrl)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.MobileHelpUrl)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.MobileTermsUrl)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.HasMany(c => c.LicenseeGameProviderConfigurations)
                .WithOne(c => c.Licensee)
                .HasForeignKey(c => c.LicenseeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Players)
                .WithOne(c => c.Licensee)
                .HasForeignKey(c => c.LicenseeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
