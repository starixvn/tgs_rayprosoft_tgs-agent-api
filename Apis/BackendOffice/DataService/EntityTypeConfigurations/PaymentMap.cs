using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Core.DataServices.Base;

namespace TGS.Apis.BackendOffice.DataService.EntityTypeConfigurations
{
    public class PaymentMap : IEntityTypeConfiguration<Payment>
    {
        void IEntityTypeConfiguration<Payment>.Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments", "dbo");

            builder.HasKey(p => p.Id);

            builder.Property(c => c.ChannelName)
                .HasColumnType(ColumnTypes.Nvarchar(50))
                .IsRequired();

            builder.Property(c => c.Reason)
                .HasColumnType(ColumnTypes.Nvarchar(4000))
                .IsRequired();

            builder.HasMany(p => p.UserPaymentAccessControls)
                .WithOne(u => u.Payment)
                .HasForeignKey(u => u.PaymentId);

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