using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class BrandGroupMap : BaseConfiguration, IEntityTypeConfiguration<BrandGroup>
    {
        void IEntityTypeConfiguration<BrandGroup>.Configure(EntityTypeBuilder<BrandGroup> builder)
        {
            builder.ToTable("BrandGroups", "dbo");
            BaseConfigureProperties<BrandGroup, Guid>(builder);

            builder.Property(e => e.BrandId)
                .IsRequired();

            builder.Property(e => e.Code)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();

            builder.HasOne(e => e.Brand)
                .WithMany(e => e.BrandGroups)
                .HasForeignKey(e => e.BrandId);
        }
    }
}
