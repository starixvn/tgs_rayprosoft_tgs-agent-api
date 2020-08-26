using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class GameMap : BaseConfiguration, IEntityTypeConfiguration<Game>
    {
        void IEntityTypeConfiguration<Game>.Configure(EntityTypeBuilder<Game> builder)
        {
            builder.ToTable("Games", "dbo");
            BaseConfigureProperties(builder);

            builder.Property(c => c.Status)
                .HasColumnType("smallint")
                .IsRequired();

            builder.Property(c => c.ExternalId)
                .HasColumnType("nvarchar(32)");

            builder.Property(c => c.Description)
                .HasColumnType("nvarchar(128)");

            builder.Property(c => c.EndpointPath)
                .HasColumnType("nvarchar(1024)");

            builder.Property(c => c.RuleUrl)
                .HasColumnType("nvarchar(1024)");

            builder.Property(c => c.GameType)
                .HasColumnType("smallint")
                .IsRequired();

            builder.Property(c => c.PlatformType)
                .HasColumnType("smallint")
                .IsRequired();
        }
    }
}
