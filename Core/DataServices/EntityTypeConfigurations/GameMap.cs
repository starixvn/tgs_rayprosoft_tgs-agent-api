using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Models;

namespace TGS.Core.DataServices.EntityTypeConfigurations
{
    internal class GameMap : BaseConfiguration, IEntityTypeConfiguration<Game>
    {
        void IEntityTypeConfiguration<Game>.Configure(EntityTypeBuilder<Game> builder)
        {
            builder.ToTable("Games", "dbo");
            BaseConfigureProperties(builder);

            builder.Property(c => c.Code)
                .HasColumnType(ColumnTypes.Nvarchar(128))
                .IsRequired();

            builder.Property(c => c.Status)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.ExternalId)
                .HasColumnType(ColumnTypes.Nvarchar(32))
                .IsRequired();

            builder.Property(c => c.Description)
                .HasColumnType(ColumnTypes.Nvarchar(128));

            builder.Property(c => c.EndpointPath)
                .HasColumnType(ColumnTypes.Nvarchar(2048));

            builder.Property(c => c.RuleUrl)
                .HasColumnType(ColumnTypes.Nvarchar(512));

            builder.Property(c => c.Type)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.PlatformType)
                .HasColumnType(ColumnTypes.SmallInt)
                .IsRequired();

            builder.Property(c => c.SubGame)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.HasTurnoverOnTieBet)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(p => p.GameProvider)
                .WithMany(p => p.Games)
                .HasForeignKey(p => p.GameProviderId);

            builder.HasIndex(c => c.GameProviderId).IncludeProperties(c => new
            {
                c.Id,
                c.Code,
                c.Name,
                c.ExternalId,
                c.PlatformType,
                c.Type,
                c.Status,
                c.Description,
                c.EndpointPath,
                c.SubGame,
                c.HasTurnoverOnTieBet,
                c.RuleUrl
            });
        }
    }
}