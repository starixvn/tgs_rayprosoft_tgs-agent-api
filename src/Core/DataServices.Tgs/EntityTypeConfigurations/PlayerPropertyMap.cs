using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.EntityTypeConfigurations
{
    internal class PlayerPropertyMap : IEntityTypeConfiguration<PlayerProperty>
    {
        public void Configure(EntityTypeBuilder<PlayerProperty> builder)
        {
            _ = builder.ToTable("player_properties");

            _ = builder.HasKey(c => new { c.PlayerCode, Type = c.Name });

            _ = builder.Property(c => c.PlayerCode)
                .HasMaxLength(16);

            _ = builder.Property(c => c.Name)
                .HasMaxLength(64);

            _ = builder.Property(c => c.Value)
                .IsRequired()
                .HasMaxLength(128);
        }
    }
}
