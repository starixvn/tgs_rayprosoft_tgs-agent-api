using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKS.Apis.LogApi.DataService.Models;

namespace TKS.Apis.LogApi.DataService.EntityTypeConfiguration
{
    public sealed class ApiLogConfiguration : IEntityTypeConfiguration<ApiLog>
    {
        void IEntityTypeConfiguration<ApiLog>.Configure(EntityTypeBuilder<ApiLog> builder)
        {
            builder.ToTable("ApiLog", "dbo")
                .HasKey(p => p.Id)
                .ForSqlServerIsClustered();

            //builder.Property(c => c.Title)
            //  .HasColumnType("nvarchar(50)")
            //  .IsRequired()
            //  .IsUnicode();
        }
    }
}
