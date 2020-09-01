using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Apis.LogApi.DataService.Models;

namespace TGS.Apis.LogApi.DataService.EntityTypeConfiguration
{
    public sealed class ServiceLogMap : IEntityTypeConfiguration<ServiceLog>
    {
        void IEntityTypeConfiguration<ServiceLog>.Configure(EntityTypeBuilder<ServiceLog> builder)
        {
            builder.ToTable("ServiceLogs", "dbo");

            builder.HasKey(p => p.Id);

            builder.Property(c => c.ServerName)
              .HasColumnType("nvarchar(64)")
              .IsRequired()
              .IsUnicode();

            builder.Property(c => c.Level)
                .HasColumnType("tinyint")
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Callsite)
                .HasColumnType("nvarchar(4000)")
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
