using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    public class PaymentMap : IEntityTypeConfiguration<Payment>
    {
        void IEntityTypeConfiguration<Payment>.Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("payments");

            builder.HasKey(p => p.Id)
                .HasName("pk_payments");

            builder.Property(c => c.Code)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(128);

            builder.Property(c => c.ExtendData)
                .HasColumnType(ColumnTypes.Jsonb);

            builder.Property(c => c.Status)
                .HasColumnType(ColumnTypes.SmallInt)
                .HasDefaultValueSql("0")
                .IsRequired();

            builder.Property(c => c.CreatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(c => c.UpdatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .IsRequired()
                .HasDefaultValueSql("Now()");

            builder.Property(c => c.UpdatedOn)
                .IsRequired()
                .HasDefaultValueSql("Now()");
        }
    }
}