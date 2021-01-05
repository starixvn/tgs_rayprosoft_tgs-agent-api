using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class LicenseeMap : BaseConfiguration, IEntityTypeConfiguration<Licensee>
    {
        void IEntityTypeConfiguration<Licensee>.Configure(EntityTypeBuilder<Licensee> builder)
        {
            builder.ToTable("Licensees", "dbo");
            BaseConfigureProperties<Licensee, Guid>(builder);

            builder.Property(c => c.Status)
                .HasColumnType(ColumnTypes.TinyInt)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasColumnType(ColumnTypes.Nvarchar(256));

            builder.Property(c => c.OperationalEmail)
                .HasColumnType(ColumnTypes.Nvarchar(128));

            builder.HasOne(p => p.Reseller)
                .WithMany(p => p.Licensees)
                .HasForeignKey(p => p.ResellerId);

            builder.HasMany(c => c.Departments)
                .WithOne(c => c.Licensees)
                .HasForeignKey(c => c.LicenseeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}