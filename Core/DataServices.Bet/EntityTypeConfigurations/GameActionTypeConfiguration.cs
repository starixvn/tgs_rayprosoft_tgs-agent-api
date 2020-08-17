using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Bet.Models;

namespace TGS.Core.DataServices.Bet.EntityTypeConfigurations
{
    public class GameActionTypeConfiguration : IEntityTypeConfiguration<GameAction>
    {
        void IEntityTypeConfiguration<GameAction>.Configure(EntityTypeBuilder<GameAction> builder)
        {
            builder.ToTable("GameActions", "dbo");
            builder.HasKey(bc => bc.Id).IsClustered(false);


        }
    }
}
