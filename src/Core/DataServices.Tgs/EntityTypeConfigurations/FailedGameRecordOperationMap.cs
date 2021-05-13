using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs.Migrations;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class FailedGameRecordOperationMap : IEntityTypeConfiguration<FailedGameRecordOperation>
    {
        void IEntityTypeConfiguration<FailedGameRecordOperation>.Configure(EntityTypeBuilder<FailedGameRecordOperation> builder)
        {
            builder.ToTable("failed_game_record_operations", Consts.BetSchema);

            builder.HasKey(bc => bc.Id)
                .HasName("pk_failed_game_record_operations");

            builder.Property(c => c.ExternalRoundId)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(c => c.GameProviderId)
                .IsRequired();

            builder.Property(c => c.PlayerCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(c => c.GameId)
                .IsRequired();

            builder.Property(c => c.ExternalId)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(c => c.Detail)
                .IsRequired();

            builder.Property(c => c.AutoStatus)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.ProcesStatus)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .IsRequired();
            builder.Property(c => c.UpdatedOn)
                .IsRequired();

            builder.HasIndex(c => new { c.GameProviderId, c.AutoStatus, c.ProcesStatus })
                .HasDatabaseName("ix_failed_game_record_operations_game_provider_id_auto_status_proces_status");

            builder.HasIndex(c => new { c.ExternalRoundId, c.PlayerCode, c.GameProviderId, c.GameId })
                .IsUnique()
                .HasDatabaseName("ux_failed_game_record_operations_external_round_id_player_code_game_provider_id_game_id");
        }
    }
}
