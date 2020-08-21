using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Apis.LogApi.DataService.Models;

namespace TGS.Apis.LogApi.DataService.EntityTypeConfiguration
{
    public sealed class EventLogTypeConfiguration : IEntityTypeConfiguration<EventLog>
    {
        void IEntityTypeConfiguration<EventLog>.Configure(EntityTypeBuilder<EventLog> builder)
        {
            builder.ToTable("EventLogs", "dbo");

            builder.HasKey(p => p.Id);

            builder.Property(c => c.Module)
              .HasColumnType("nvarchar(32)")
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
