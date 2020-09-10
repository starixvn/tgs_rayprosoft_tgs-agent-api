using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Apis.LogApi.DataService.Models;
using TGS.Core.DataServices.Base;

namespace TGS.Apis.LogApi.DataService.EntityTypeConfiguration
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
                .HasColumnType(ColumnTypes.Nvarchar(30))
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
