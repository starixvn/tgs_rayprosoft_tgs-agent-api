using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class LicenseeGameProviderMap : IEntityTypeConfiguration<LicenseeGameProvider>
    {
        void IEntityTypeConfiguration<LicenseeGameProvider>.Configure(EntityTypeBuilder<LicenseeGameProvider> builder)
        {
            builder.ToTable("licensee_game_providers");

            builder.HasKey(e => new {e.LicenseeId, e.GameProviderId})
                .HasName("pk_licensee_game_providers");

            builder
                .HasOne(e => e.GameProvider)
                .WithMany(e => e.Licensees)
                .HasForeignKey(e => e.GameProviderId)
                .HasConstraintName("fk_licensee_game_providers_game_providers_game_provider_id");

            builder.HasIndex(e => e.GameProviderId)
                .HasDatabaseName("ix_licensee_game_providers_game_provider_id");

            builder
                .HasOne(e => e.Licensee)
                .WithMany(e => e.GameProviders)
                .HasForeignKey(e => e.LicenseeId)
                .HasConstraintName("fk_licensee_game_providers_licensees_licensee_id");
            ;
        }
    }
}