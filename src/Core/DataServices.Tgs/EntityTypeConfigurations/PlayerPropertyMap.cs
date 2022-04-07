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

            _ = builder.HasKey(c => new { c.BrandId, c.PlayerCode, c.Name });

            _ = builder.Property(c => c.PlayerCode)
                .HasMaxLength(16);

            _ = builder.Property(c => c.BrandId)
                .IsRequired();

            _ = builder.Property(c => c.Name)
                .HasMaxLength(256);

            _ = builder.Property(c => c.Value)
                .IsRequired()
                .HasMaxLength(512);

            _ = builder.HasOne<Brand>()
                 .WithMany()
                 .HasForeignKey(c => c.BrandId)
                 .HasConstraintName("fk_brand_player_property_brand_id")
                 .OnDelete(DeleteBehavior.Cascade);

            _ = builder.HasOne<Player>()
                .WithMany()
                .HasForeignKey(c => c.PlayerCode)
                .HasConstraintName("fk_player_player_property_player_code")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
