using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Bet.Models;

namespace TGS.Core.DataServices.Bet.EntityTypeConfigurations
{
    internal class GameActionBatchMap : IEntityTypeConfiguration<GameActionBatch>
    {
        void IEntityTypeConfiguration<GameActionBatch>.Configure(EntityTypeBuilder<GameActionBatch> builder)
        {
            builder.ToTable("GameActionBatches", "dbo");
            builder.HasKey(bc => bc.Id).IsClustered(false); ;

            builder.Property(c => c.ErrorType)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.TransactionsCount)
                .HasColumnType(ColumnTypes.Int)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .HasColumnType(ColumnTypes.DatetimeOffset)
                .IsRequired();

            builder.Property(c => c.ExternalId)
                .HasColumnType(ColumnTypes.Nvarchar(64))
                .IsRequired();

            builder.Property(c => c.ErrorDescription)
                .HasColumnType(ColumnTypes.Nvarchar(1024));

            builder.Property(c => c.GameProviderConfigurationId)
                .IsRequired();

            builder.Property(c => c.ErrorsJson)
                .HasColumnType(ColumnTypes.Nvarchar(4000));

            builder.HasMany(p => p.GameActions)
                .WithOne(p => p.GameActionBatch)
                .HasForeignKey(p => p.GameActionBatchId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
