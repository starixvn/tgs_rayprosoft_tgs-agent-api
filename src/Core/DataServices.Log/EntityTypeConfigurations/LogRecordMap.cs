using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Log.Models;

namespace TGS.Core.DataServices.Log.EntityTypeConfigurations
{
    public sealed class LogRecordMap : IEntityTypeConfiguration<LogRecord>
    {
        void IEntityTypeConfiguration<LogRecord>.Configure(EntityTypeBuilder<LogRecord> builder)
        {
            builder.ToTable("log_records", "dbo");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.ServerName)
                .HasMaxLength(128)
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Level)
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Message)
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Exception)
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.RecordedOn)
                .IsRequired();
        }
    }
}