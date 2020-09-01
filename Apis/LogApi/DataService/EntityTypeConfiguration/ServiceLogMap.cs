using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Apis.LogApi.DataService.Models;
using TGS.Core.DataServices.Base;

namespace TGS.Apis.LogApi.DataService.EntityTypeConfiguration
{
    public sealed class ServiceLogMap : IEntityTypeConfiguration<ServiceLog>
    {
        void IEntityTypeConfiguration<ServiceLog>.Configure(EntityTypeBuilder<ServiceLog> builder)
        {
            builder.ToTable("ServiceLogs", "dbo");

            builder.HasKey(p => p.Id);

            builder.Property(c => c.MachineName)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.Level)
                .HasColumnType(ColumnTypes.Nvarchar(10));

            builder.Property(c => c.Callsite)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.ServerAddress)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.RemoteAddress)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.Logger)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.Port)
                .HasColumnType(ColumnTypes.Nvarchar(255));

            builder.Property(c => c.Properties)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.Message)
                .HasColumnType(ColumnTypes.Nvarchar(4000));

            builder.Property(c => c.Exception)
                .HasColumnType(ColumnTypes.Nvarchar(4000));

            builder.Property(c => c.RecordedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset);
        }
    }
}
