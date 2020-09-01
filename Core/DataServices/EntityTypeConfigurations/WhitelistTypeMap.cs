using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class WhitelistTypeMap : IEntityTypeConfiguration<Whitelist>
    {
        void IEntityTypeConfiguration<Whitelist>.Configure(EntityTypeBuilder<Whitelist> builder)
        {
            builder.ToTable("Whitelists", "dbo");
            builder.HasKey(bc => bc.Id);

            builder.Property(c => c.IpAddress)
                .HasColumnType(ColumnTypes.Nvarchar(64))
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Description)
                .HasColumnType(ColumnTypes.Nvarchar(256))
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.InternalNote)
                .HasColumnType(ColumnTypes.Nvarchar(256))
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.CreatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(256))
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.UpdatedByUsername)
                .HasColumnType(ColumnTypes.Nvarchar(256))
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.CreatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(c => c.UpdatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();
        }
    }
}
