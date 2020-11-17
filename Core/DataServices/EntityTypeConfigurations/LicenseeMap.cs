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
                .HasColumnType(ColumnTypes.Varchar(32))
                .IsRequired();

            builder.Property(c => c.ApiKey)
                .HasColumnType(ColumnTypes.Varchar(64))
                .IsRequired();

            builder.Property(c => c.TimeZoneId)
                .HasColumnType(ColumnTypes.Nvarchar(256));

            builder.HasMany(c => c.GameProviderConfigurations)
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
