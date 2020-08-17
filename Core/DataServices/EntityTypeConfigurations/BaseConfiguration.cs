using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    public class BaseConfiguration
    {
        protected void BaseConfigureProperties<T>(EntityTypeBuilder<T> builder) where T : BaseEntity
        {
            builder.HasKey(p => p.Id).IsClustered(false); ;

            builder.Property(c => c.Code)
                .HasColumnType("nvarchar(10)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Name)
                .HasColumnType("nvarchar(30)")
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
