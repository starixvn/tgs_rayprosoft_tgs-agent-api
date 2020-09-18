using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class StationLetterPlayerMap : IEntityTypeConfiguration<StationLetterPlayer>
    {
        void IEntityTypeConfiguration<StationLetterPlayer>.Configure(EntityTypeBuilder<StationLetterPlayer> builder)
        {
            builder.ToTable("StationLetterPlayers", "dbo");
            builder.HasKey(c => new { c.StationLetterId, c.PlayerCode });
        }
    }
}
