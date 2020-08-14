using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfiguration
{
    public class WhitelistTypeConfiguration : IEntityTypeConfiguration<Whitelist>
    {
        void IEntityTypeConfiguration<Whitelist>.Configure(EntityTypeBuilder<Whitelist> builder)
        {
            builder.ToTable("Whitelists", "dbo");
            builder.HasKey(bc => bc.Id);

            builder.Property(c => c.IpAddress)
                .HasColumnType("nvarchar(64)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Description)
                .HasColumnType("nvarchar(256)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.InternalNote)
                .HasColumnType("nvarchar(256)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.CreatedByUsername)
                .HasColumnType("nvarchar(256)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.UpdatedByUsername)
                .HasColumnType("nvarchar(256)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.CreatedOn)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            builder.Property(c => c.UpdatedOn)
                .HasColumnType("datetimeoffset")
                .IsRequired();
        }
    }
}
