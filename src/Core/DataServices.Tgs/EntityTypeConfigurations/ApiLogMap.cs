using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Migrations;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    public sealed class ApiLogMap : IEntityTypeConfiguration<ApiLog>
    {
        void IEntityTypeConfiguration<ApiLog>.Configure(EntityTypeBuilder<ApiLog> builder)
        {
            builder.ToTable("api_logs", Consts.LogSchema);

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
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Method)
                .HasMaxLength(256)
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.Prefix)
                .HasMaxLength(256)
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.RequestHeaders)
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.RequestContent)
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.ResponseHeaders)
                .IsRequired()
                .IsUnicode();

            builder.Property(c => c.ResponseContent)
                .IsRequired()
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