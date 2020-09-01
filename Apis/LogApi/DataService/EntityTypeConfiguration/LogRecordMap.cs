using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Apis.LogApi.DataService.Models;
using TGS.Core.DataServices.Base;

namespace TGS.Apis.LogApi.DataService.EntityTypeConfiguration
{
    public sealed class LogRecordMap : IEntityTypeConfiguration<LogRecord>
    {
        void IEntityTypeConfiguration<LogRecord>.Configure(EntityTypeBuilder<LogRecord> builder)
        {
            builder.ToTable("LogRecords", "dbo");

            builder.HasKey(p => p.Id);

            builder.Property(c => c.ServerName)
              .HasColumnType(ColumnTypes.Nvarchar(64))
              .IsRequired()
              .IsUnicode();

            builder.Property(c => c.Level)
                .HasColumnType(ColumnTypes.TinyInt)
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Message)
                .HasColumnType(ColumnTypes.Nvarchar(512))
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Exception)
                .HasColumnType(ColumnTypes.Nvarchar(4000))
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.RecordedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();
        }
    }
}
