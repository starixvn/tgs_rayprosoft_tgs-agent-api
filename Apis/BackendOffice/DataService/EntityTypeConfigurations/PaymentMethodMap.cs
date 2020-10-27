using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Core.DataServices.Base;

namespace TGS.Apis.BackendOffice.DataService.EntityTypeConfigurations
{
    public class PaymentMethodMap : IEntityTypeConfiguration<PaymentMethod>
    {
        void IEntityTypeConfiguration<PaymentMethod>.Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.ToTable("PaymentMethods", "dbo");

            builder.HasKey(p => p.Id);

            builder.Property(c => c.Code)
                .HasColumnType(ColumnTypes.Nvarchar(50))
                .IsRequired();

            builder.Property(c => c.Description)
                .HasColumnType(ColumnTypes.Nvarchar(128))
                .IsRequired();

            builder.HasMany(p => p.PaymentChannels)
                .WithOne(p => p.PaymentMethod)
                .HasForeignKey(p => p.PaymentMethodId);

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