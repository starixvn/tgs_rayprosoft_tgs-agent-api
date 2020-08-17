using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Bet.Models;

namespace TGS.Core.DataServices.Bet.EntityTypeConfigurations
{
    public class GameActionBatchTypeConfiguration : IEntityTypeConfiguration<GameActionBatch>
    {
        void IEntityTypeConfiguration<GameActionBatch>.Configure(EntityTypeBuilder<GameActionBatch> builder)
        {
            builder.ToTable("GameActionBatches", "dbo");
            builder.HasKey(bc => bc.Id).IsClustered(false); ;

            builder.Property(c => c.ErrorType)
                .HasColumnType("smallint")
                .IsRequired();

            builder.Property(c => c.TransactionsCount)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .HasColumnType("datetimeoffset(7)")
                .IsRequired();

            builder.Property(c => c.ExternalId)
                .HasColumnType("nvarchar(64)")
                .IsRequired();

            builder.Property(c => c.ErrorDescription)
                .HasColumnType("nvarchar(1024)")
                .IsRequired(false);

            builder.Property(c => c.GameProviderConfiguration)
                .HasColumnType("nvarchar(4000)")
                .IsRequired();

            builder.Property(c => c.ErrorsJson)
                .HasColumnType("nvarchar(4000)")
                .IsRequired(false);

            builder.HasMany(p => p.GameActions)
                .WithOne(p => p.GameActionBatch)
                .HasForeignKey(p => p.GameActionBatchId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
