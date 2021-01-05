using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class VipLevelMap : BaseConfiguration, IEntityTypeConfiguration<VipLevel>
    {
        void IEntityTypeConfiguration<VipLevel>.Configure(EntityTypeBuilder<VipLevel> builder)
        {
            builder.ToTable("VipLevels", "dbo");
            BaseConfigureProperties<VipLevel, Guid>(builder);

            builder.Property(c => c.Code)
                .HasColumnType(ColumnTypes.Varchar(16))
                .IsRequired();

            builder.Property(c => c.Name)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();

            builder.Property(c => c.CreatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();

            builder.Property(c => c.UpdatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(c => c.UpdatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.HasMany(p => p.Players)
                .WithOne(p => p.Vip)
                .HasForeignKey(p => p.VipId);
        }
    }
}