using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class
        BrandGameProviderScheduledMap : IEntityTypeConfiguration<
            BrandGameProviderSchedule>
    {
        void IEntityTypeConfiguration<BrandGameProviderSchedule>.Configure(
            EntityTypeBuilder<BrandGameProviderSchedule> builder)
        {
            builder.ToTable("BrandGameProviderSchedules", "dbo");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Status)
                .HasColumnType(ColumnTypes.TinyInt)
                .IsRequired();

            builder.HasOne(s => s.BrandGameProvider)
                .WithMany(t => t.Schedules)
                .HasForeignKey(t => t.BrandGameProviderId);
        }
    }
}