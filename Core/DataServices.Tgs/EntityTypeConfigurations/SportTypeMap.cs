using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class SportTypeMap : IEntityTypeConfiguration<SportType>
    {
        void IEntityTypeConfiguration<SportType>.Configure(EntityTypeBuilder<SportType> builder)
        {
            builder.ToTable("sport_types");

            builder.HasKey(e => e.Id)
                .HasName("pk_sport_types");

            builder.HasAlternateKey(e => new {e.GameProviderId, e.Code})
                .HasName("ak_sport_types_game_provider_id_code");

            builder.Property(e => e.Code)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(e => e.ExternalId)
                .HasMaxLength(16)
                .IsRequired();

            builder.HasOne(e => e.GameProvider)
                .WithMany(e => e.SportTypes)
                .HasForeignKey(e => e.GameProviderId)
                .HasConstraintName("fk_sport_types_game_providers_game_provider_id");

            builder.HasIndex(e => e.GameProviderId)
                .HasDatabaseName("ix_sport_types_game_provider_id");
        }
    }
}