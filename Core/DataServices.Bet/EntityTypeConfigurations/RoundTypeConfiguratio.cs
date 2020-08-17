using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Bet.Models;

namespace TGS.Core.DataServices.Bet.EntityTypeConfigurations
{
    public class RoundTypeConfiguration : IEntityTypeConfiguration<Round>
    {
        void IEntityTypeConfiguration<Round>.Configure(EntityTypeBuilder<Round> builder)
        {
            builder.ToTable("Rounds", "dbo");
            builder.HasKey(bc => bc.Id).IsClustered(false);


        }
    }
}
