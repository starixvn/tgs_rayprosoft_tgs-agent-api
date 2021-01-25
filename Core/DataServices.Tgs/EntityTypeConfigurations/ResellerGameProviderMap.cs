using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class ResellerGameProviderMap : IEntityTypeConfiguration<ResellerGameProvider>
    {
        void IEntityTypeConfiguration<ResellerGameProvider>.Configure(EntityTypeBuilder<ResellerGameProvider> builder)
        {
            builder.ToTable("reseller_game_providers");

            builder.HasKey(e => new {e.ResellerId, e.GameProviderId})
                .HasName("pk_reseller_game_providers");

            builder
                .HasOne(e => e.GameProvider)
                .WithMany(e => e.Resellers)
                .HasForeignKey(e => e.GameProviderId)
                .HasConstraintName("fk_reseller_game_providers_game_providers_game_provider_id");

            builder.HasIndex(e => e.GameProviderId)
                .HasDatabaseName("ix_reseller_game_providers_game_provider_id");

            builder
                .HasOne(e => e.Reseller)
                .WithMany(e => e.GameProviders)
                .HasForeignKey(e => e.ResellerId)
                .HasConstraintName("fk_reseller_game_providers_game_resellers_reseller_id");
        }
    }
}