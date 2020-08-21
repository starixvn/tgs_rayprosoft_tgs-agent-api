using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Apis.LogApi.DataService.Models;

namespace TGS.Apis.LogApi.DataService.EntityTypeConfiguration
{
    public sealed class OsLogTypeConfiguration : IEntityTypeConfiguration<OsLog>
    {
        void IEntityTypeConfiguration<OsLog>.Configure(EntityTypeBuilder<OsLog> builder)
        {
            builder.ToTable("OsLogs", "dbo");

            builder.HasKey(p => p.Id);

            builder.Property(c => c.ServerName)
              .HasColumnType("nvarchar(64)")
              .IsRequired()
              .IsUnicode();

            builder.Property(c => c.ServiceName)
                .HasColumnType("nvarchar(64)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Message)
                .HasColumnType("nvarchar(512)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Exception)
                .HasColumnType("nvarchar(4000)")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.RecordedOn)
                .HasColumnType("datetimeoffset")
                .IsRequired();
        }
    }
}
