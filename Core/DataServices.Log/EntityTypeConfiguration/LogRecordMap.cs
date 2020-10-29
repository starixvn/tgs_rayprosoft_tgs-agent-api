using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Log.Models;

namespace TGS.Core.DataServices.Log.EntityTypeConfiguration
{
    public sealed class LogRecordMap : IEntityTypeConfiguration<LogRecord>
    {
        void IEntityTypeConfiguration<LogRecord>.Configure(EntityTypeBuilder<LogRecord> builder)
        {
            builder.ToTable("LogRecords", "dbo");

            builder.HasKey(p => p.RecordedOn);

            builder.Property(c => c.ServerName)
              .HasColumnType(ColumnTypes.Nvarchar(128))
              .IsRequired()
              .IsUnicode();

            builder.Property(c => c.Level)
                .HasColumnType(ColumnTypes.Int)
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
