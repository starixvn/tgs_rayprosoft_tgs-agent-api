using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class GameProviderConfigurationMap : IEntityTypeConfiguration<GameProviderConfiguration>
    {
        void IEntityTypeConfiguration<GameProviderConfiguration>.Configure(
            EntityTypeBuilder<GameProviderConfiguration> builder)
        {
            builder.ToTable("game_provider_configurations");

            builder.HasKey(p => p.Id)
                .HasName("pk_game_provider_configurations");

            builder.Property(p => p.Name)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(256);

            builder.Property(c => c.ClientId)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(c => c.SecretKey)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(c => c.GameProviderId)
                .IsRequired();

            builder.Property(c => c.Status)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.DesktopEndpoint)
                .HasMaxLength(1024)
                .IsRequired();

            builder.Property(c => c.MobileEndpoint)
                .HasMaxLength(1024)
                .IsRequired();

            builder.Property(c => c.ConfigurationData)
                .IsRequired();

            builder.Property(c => c.IsDefault)
                .IsRequired();

            builder.Property(c => c.CreatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(c => c.UpdatedByUsername)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .IsRequired();

            builder.Property(c => c.UpdatedOn)
                .IsRequired();

            builder.HasOne(p => p.GameProvider)
                .WithMany(p => p.GameProviderConfigurations)
                .HasForeignKey(p => p.GameProviderId)
                .HasConstraintName("fk_game_provider_configurations_game_providers_game_provider_id");

            builder.HasIndex(e => e.GameProviderId)
                .HasDatabaseName("ix_game_provider_configurations_game_provider_id");
        }
    }
}