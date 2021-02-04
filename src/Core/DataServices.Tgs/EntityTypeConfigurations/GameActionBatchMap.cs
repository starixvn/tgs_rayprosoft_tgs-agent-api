using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Migrations;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class GameActionBatchMap : IEntityTypeConfiguration<GameActionBatch>
    {
        void IEntityTypeConfiguration<GameActionBatch>.Configure(EntityTypeBuilder<GameActionBatch> builder)
        {
            builder.ToTable("game_action_batches", Consts.BetSchema);

            builder.HasKey(bc => bc.Id)
                .HasName("pk_game_action_batches");

            builder.Property(c => c.ErrorType)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.TransactionsCount)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .IsRequired();

            builder.Property(c => c.ExternalId)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(c => c.ErrorDescription)
                .HasMaxLength(1024);

            builder.Property(c => c.GameProviderConfigurationId)
                .IsRequired();

            builder.Property(c => c.ErrorsJson)
                .HasColumnType(ColumnTypes.Jsonb);

            builder.HasOne(c => c.GameProviderConfiguration)
                .WithMany()
                .HasForeignKey(c => c.GameProviderConfigurationId)
                .HasConstraintName(
                    "fk_game_action_batches_game_provider_configurations_game_provider_configuration_id");

            builder.HasIndex(c => c.GameProviderConfigurationId)
                .HasDatabaseName("ix_game_action_batches_game_provider_configuration_id");
        }
    }
}