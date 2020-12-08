using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class
        BrandGameProviderScheduledMap : IEntityTypeConfiguration<
            BrandGameProviderSchedule>
    {
        void IEntityTypeConfiguration<BrandGameProviderSchedule>.Configure(
            EntityTypeBuilder<BrandGameProviderSchedule> builder)
        {
            builder.ToTable("BrandGameProviderSchedules", "dbo");

            builder.Property(c => c.Status)
                .HasColumnType(ColumnTypes.TinyInt)
                .IsRequired();

            builder.HasOne(bgp => bgp.BrandGameProvider)
                .WithMany(t => t.Schedules)
                .HasForeignKey(t => t.BrandGameProviderId);
        }
    }
}