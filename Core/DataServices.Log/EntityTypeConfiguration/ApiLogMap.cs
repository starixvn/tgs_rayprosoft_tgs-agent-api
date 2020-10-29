using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Log.Models;

namespace TGS.Core.DataServices.Log.EntityTypeConfiguration
{
    public sealed class ApiLogMap : IEntityTypeConfiguration<ApiLog>
    {
        void IEntityTypeConfiguration<ApiLog>.Configure(EntityTypeBuilder<ApiLog> builder)
        {
            builder.ToTable("ApiLogs", "dbo");

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
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Method)
                .HasColumnType(ColumnTypes.Nvarchar(256))
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Prefix)
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
