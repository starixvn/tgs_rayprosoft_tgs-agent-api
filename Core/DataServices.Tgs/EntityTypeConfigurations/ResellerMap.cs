using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class ResellerMap : BaseConfiguration, IEntityTypeConfiguration<Reseller>
    {
        void IEntityTypeConfiguration<Reseller>.Configure(EntityTypeBuilder<Reseller> builder)
        {
            builder.ToTable("Resellers", "dbo");
            BaseConfigureProperties<Reseller, Guid>(builder);

            builder.Property(c => c.Status)
                .HasColumnType(ColumnTypes.TinyInt)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasColumnType(ColumnTypes.Nvarchar(256));

            builder.Property(c => c.OperationalEmail)
                .HasColumnType(ColumnTypes.Nvarchar(128));

            builder.Property(e => e.BusinessType)
                .HasColumnType(ColumnTypes.TinyInt)
                .IsRequired();

            builder.HasMany(c => c.Departments)
                .WithOne(c => c.Reseller)
                .HasForeignKey(c => c.ResellerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}