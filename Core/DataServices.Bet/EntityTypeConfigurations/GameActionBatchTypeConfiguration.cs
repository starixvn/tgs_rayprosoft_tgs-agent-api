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


        }
    }
}
