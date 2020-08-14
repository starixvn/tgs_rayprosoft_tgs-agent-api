using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfiguration
{
    public class ResellerGameProviderTypeConfiguration : IEntityTypeConfiguration<ResellerGameProvider>
    {
        void IEntityTypeConfiguration<ResellerGameProvider>.Configure(EntityTypeBuilder<ResellerGameProvider> builder)
        {
            builder.ToTable("ResellerGameProviders", "dbo");

            builder.HasKey(bc => new { bc.ResellerId, bc.GameProviderId });
            builder
                .HasOne(bc => bc.GameProvider)
                .WithMany(c => c.Resellers)
                .HasForeignKey(bc => bc.GameProviderId);
            builder
                .HasOne(bc => bc.Reseller)
                .WithMany(c => c.GameProviders)
                .HasForeignKey(bc => bc.ResellerId);
        }
    }
}
