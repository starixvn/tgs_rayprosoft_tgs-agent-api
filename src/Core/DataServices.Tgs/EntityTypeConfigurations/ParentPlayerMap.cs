using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class ParentPlayerMap : IEntityTypeConfiguration<ParentPlayer>
    {
        void IEntityTypeConfiguration<ParentPlayer>.Configure(EntityTypeBuilder<ParentPlayer> builder)
        {
            builder.ToTable("parent_players");

            builder.HasKey(bc => bc.PlayerCode)
                .HasName("pk_player_code");

            builder.Property(c => c.PlayerCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(c => c.ParentPlayerCode)
                .HasMaxLength(16)
                .IsRequired();
        }
    }
}