using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Log.Models;

namespace TGS.Core.DataServices.Log.EntityTypeConfiguration
{
    public sealed class AdminLogMap : IEntityTypeConfiguration<AdminLog>
    {
        void IEntityTypeConfiguration<AdminLog>.Configure(EntityTypeBuilder<AdminLog> builder)
        {
            builder.ToTable("AdminLogs", "dbo");

            builder.HasKey(p => p.RecordedOn);

            builder.Property(c => c.Ip)
              .HasColumnType(ColumnTypes.Nvarchar(50))
              .IsRequired()
              .IsUnicode();

            builder.Property(c => c.ServerName)
                .HasColumnType(ColumnTypes.Nvarchar(128))
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Path)
                .HasColumnType(ColumnTypes.Nvarchar(256))
                .IsRequired(false)
                .IsUnicode();

            builder.Property(c => c.Method)
                .HasColumnType(ColumnTypes.Nvarchar(256))
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Description)
                .HasColumnType(ColumnTypes.Nvarchar(256))
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.RequestHeaders)
                .HasColumnType(ColumnTypes.Nvarchar(4000))
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.RequestContent)
                .HasColumnType(ColumnTypes.Nvarchar(4000))
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.ResponseHeaders)
                .HasColumnType(ColumnTypes.Nvarchar(4000))
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.ResponseContent)
                .HasColumnType(ColumnTypes.Nvarchar(4000))
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Exception)
                .HasColumnType(ColumnTypes.Nvarchar(4000))
                .IsUnicode();

            builder.Property(c => c.StackTrace)
                .HasColumnType(ColumnTypes.Nvarchar(4000))
                .IsUnicode();

            builder.Property(c => c.ResponseCode)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.ExecutionTimeMs)
                .HasColumnType(ColumnTypes.Int);

            builder.Property(c => c.RecordedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();
        }
    }
}
