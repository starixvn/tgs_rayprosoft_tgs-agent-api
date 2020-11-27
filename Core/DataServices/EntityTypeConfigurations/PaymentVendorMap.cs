using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    public class PaymentVendorMap : IEntityTypeConfiguration<PaymentVendor>
    {
        void IEntityTypeConfiguration<PaymentVendor>.Configure(EntityTypeBuilder<PaymentVendor> builder)
        {
            builder.ToTable("PaymentVendors", "dbo");

            builder.HasKey(p => p.Id);

            builder.Property(c => c.Code)
                .HasColumnType(ColumnTypes.Nvarchar(50))
                .IsRequired();

            builder.Property(c => c.Description)
                .HasColumnType(ColumnTypes.Nvarchar(128));

            builder.HasMany(p => p.PaymentChannels)
                .WithOne(u => u.PaymentVendor)
                .HasForeignKey(p => p.PaymentVendorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.PaymentMethods)
                .WithOne(p => p.PaymentVendor)
                .HasForeignKey(p => p.PaymentVendorId)
                .OnDelete(DeleteBehavior.Restrict);

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
        }
    }
}