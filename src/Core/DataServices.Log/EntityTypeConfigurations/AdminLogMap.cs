using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Log.Models;

namespace TGS.Core.DataServices.Log.EntityTypeConfigurations
{
    public sealed class AdminLogMap : IEntityTypeConfiguration<AdminLog>
    {
        void IEntityTypeConfiguration<AdminLog>.Configure(EntityTypeBuilder<AdminLog> builder)
        {
            builder.ToTable("admin_logs", "dbo");

            builder.HasKey(p => p.Id);

            builder.Property(c => c.Ip)
                .HasMaxLength(50)
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.ServerName)
                .HasMaxLength(128)
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Path)
                .HasMaxLength(256)
                .IsRequired(false)
                .IsUnicode();

            builder.Property(c => c.Method)
                .HasMaxLength(256)
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Description)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(c => c.RequestHeaders)
                .IsRequired();

            builder.Property(c => c.RequestContent)
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.ResponseHeaders)
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.ResponseContent)
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Exception)
                .IsUnicode();

            builder.Property(c => c.StackTrace)
                .IsUnicode();

            builder.Property(c => c.ResponseCode)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.ExecutionTimeMs);

            builder.Property(c => c.RecordedOn)
                .IsRequired();
        }
    }
}